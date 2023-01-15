using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Server.Filer.Utility
{
    internal class Value
    {
        private static List<Exception> CompareItem<C>(in C obj1, in C obj2, in C standards, out C result, in bool overWrite = false)
        {
            result = standards;
            var properties = typeof(C).GetProperties();
            foreach (var property in properties)
            {
                if ((property.GetValue(obj1) as IEnumerator<object>)  != null|| (property.GetValue(obj1) as IEnumerable<object>) != null )
                {
                    dynamic a;
                    CompareItemList(obj1, obj2, standards, out a, overWrite);
                    property.SetValue(result, a);
                }
                else
                {
                //ファイルの内容と更新内容の変化があった場合
                    if (property.GetValue(obj1) != property.GetValue(standards) || property.GetValue(obj2) != property.GetValue(standards))
                    {
                        if (property.GetValue(obj1) != property.GetValue(obj2))
                        {
                            //ファイル内の内容と更新前内容に変化があった場合と、上書き保存を禁止された場合
                            if (!overWrite || property.GetValue(obj2) != property.GetValue(standards))
                            {
                                throw new InvalidOperationException();
                            }
                            //変更箇所ではなかった場合
                            else if (property.GetValue(obj1) == property.GetValue(standards))
                            {
                                property.SetValue(result, property.GetValue(obj2));
                            }
                        }
                    }
                }
            }
            var fields = typeof(C).GetFields();
            foreach (var field in fields)
            {
                //ファイルの内容と更新内容の変化があった場合
                if (field.GetValue(obj1) != field.GetValue(standards) || field.GetValue(obj2) != field.GetValue(standards))
                {
                    if (field.GetValue(obj1) != field.GetValue(obj2))
                    {
                        //ファイル内の内容と更新前内容に変化があった場合と、上書き保存を禁止された場合
                        if (!overWrite || field.GetValue(obj2) != field.GetValue(standards))
                        {
                            throw new InvalidOperationException();
                        }
                        //変更箇所ではなかった場合
                        else if (field.GetValue(obj1) == field.GetValue(standards))
                        {
                            field.SetValue(result, field.GetValue(obj2));
                        }
                    }
                }
            }
        }
        private static List<Exception> CompareItem<C>(in C obj1, in C obj2, ref C standards, in bool overWrite = false)
        {
            C result;
            var a = CompareItem<C>(obj1, obj2, standards, out result, overWrite);
            standards = result;
            return a;
        }
        private static List<Exception> CompareItemList<C>(in C? obj1, in C? obj2, in C? standards, out C result, in bool overWrite = false,in Primary primary = Primary.obj1)
        {
            result = default(C);
            var exceptions = new List<Exception>();
            IEnumerator<object> enumerator1;
            IEnumerator<object> enumerator2;
            IEnumerator<object> enumeratorStandards;
            //型チェック(obj1)
            if ((obj1 as IEnumerator<object>) != null)
            {
                enumerator1 = ((IEnumerator<object>)obj1);
            }
            else if((obj1 as IEnumerable<object>) != null)
            {
                enumerator1 = ((IEnumerable<object>)obj1).GetEnumerator();
            }
            else
            {
                exceptions.Add(new ArgumentException());
                goto InError;
            }
            //型チェック(obj2)
            if ((obj2 as IEnumerator<object>) != null)
            {
                enumerator2 = ((IEnumerator<object>)obj2);
            }
            else if ((obj2 as IEnumerable<object>) != null)
            {
                enumerator2 = ((IEnumerable<object>)obj2).GetEnumerator();
            }
            else
            {
                exceptions.Add(new ArgumentException());
                goto InError;
            }
            //型チェック(standards)
            if ((standards as IEnumerator<object>) != null)
            {
                enumeratorStandards = ((IEnumerator<object>)standards);
                goto Compare;
            }
            else if ((standards as IEnumerable<object>) != null)
            {
                enumeratorStandards = ((IEnumerable<object>)standards).GetEnumerator();
                goto Compare;
            }
            else
            {
                exceptions.Add(new ArgumentException());
                goto InError;
            }
            //型変換がおかしくなった時
            InError:
                result = default(C)!;
                return exceptions;
        //比較フェーズ ※IEnumeratorで扱うため、糖衣構文であるForEach構文が使えないので注意
        Compare:
            LinkedList<object> resultList = new();
            int index1=0;
            int index2=0;
            //総数カウント
            while (enumerator1.MoveNext())
            {
                index1++;
            }
            //総数カウント
            while (enumerator2.MoveNext())
            {
                index2++;
            }
            //ポインタリストを作成。(変数,ポインタ)タプルの形。
            (int,object)[] positions1 = new (int, object)[index1];
            (int, object)[] positions2 = new (int, object)[index2];
            LinkedList<(int, object)> addition1 = new();
            enumerator1.Reset();
            enumerator2.Reset();
            int current = 1;
            //ポインタリストの設定
            while (enumerator1.MoveNext())
            {
                int pointer = -1;
                while (enumeratorStandards.MoveNext())
                {
                    if(enumerator1.Current == enumeratorStandards.Current)
                    {
                        positions1[current] = (pointer, enumerator1.Current);
                        break;
                    }
                    pointer++;
                }
                //もしなければ、
                if(pointer == -1)
                {
                    //アンカー取得
                    var befor = positions1[current - 1].Item1;
                    foreach (var item in addition1)
                    {
                        if(item.Item1 > befor)
                        {
                            var a = new LinkedListNode<(int, object)>((befor, enumerator1.Current));
                            if (addition1.Find(item) != null)
                            {
                                addition1.AddBefore(addition1.Find(item), a);
                            }
                            else
                            {
                                addition1.AddLast(a);
                            }
                        }
                    }
                }
                enumeratorStandards.Reset();
                current++;
            }
            //obj2にも同じことを
            while (enumerator2.MoveNext())
            {
                int pointer = -1;
                while (enumeratorStandards.MoveNext())
                {
                    if (enumerator2.Current == enumeratorStandards.Current)
                    {
                        positions2[current] = (pointer, enumerator2.Current);
                        break;
                    }
                    pointer++;
                }
                //もしなければ、
                if (pointer == -1)
                {
                    //アンカー取得
                    var befor = positions1[current - 1].Item1;
                    foreach (var item in addition1)
                    {
                        if (item.Item1 > befor)
                        {
                            var a = new LinkedListNode<(int, object)>((befor, enumerator2.Current));
                            if (addition2.Find(item) != null)
                            {
                                addition2.AddBefore(addition2.Find(item), a);
                            }
                            else
                            {
                                addition2.AddLast(a);
                            }
                        }
                    }
                }
                enumeratorStandards.Reset();
                current++;
            }
            //配列決定基準配列の作成
            int sCount = 0;
            while (enumeratorStandards.MoveNext())
            {
                sCount++;
            }
            var standerdArray = new object[sCount];
            enumeratorStandards.Reset();
            sCount = 0;
            while (enumeratorStandards.MoveNext())
            {
                standerdArray[sCount] = enumeratorStandards.Current;
                sCount++;
            }
            enumeratorStandards.Reset();
            //結果反映用の関数を作成
            var append = (in object element) =>
            {
                int distance = 0;
                int aim = Array.IndexOf(standerdArray,element);
                while (aim > 0)
                {

                }
                enumeratorStandards.Reset();
            };
            //比較。Sortし続ける?
            foreach (var item in positions1)
            {
                foreach (var item2 in positions2)
                {
                    if(item.Item1 == item2.Item1)
                    {
                        if(item.Item2 == item2.Item2)
                        {
                            foreach (var feched in resultList)
                            {

                            }
                        }
                    }
                }
            }
        }

        enum Primary
        {
            obj1, // obj1 > obj2 > standerd
            obj2,//  obj2 > obj1 > standerd
            standerd,// standerd > obj1 > obj2
        }
    }
}
