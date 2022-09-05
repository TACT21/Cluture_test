
サーバーセットアップ
SSHの公開鍵生成は↓<br>
https://click.jp/knowledge/1476/<br>
公開鍵については↓<br>
https://nishiohirokazu.hatenadiary.org/entry/20140809/1407556873
<br>参考↓<br>
https://qiita.com/kazokmr/items/754169cfa996b24fcbf5

pacman -Syu openssh

SSHさーばーオートアップ化
```
systemctl enable sshd
```

SSHの鍵ファイル作成。　ここ(~/.ssh/authorized_keys)にpubファイルの鍵を入れ込む。
```
mkdir ~/.ssh
rm -f ~/id_rsa.pub
chmod 700 ~/.ssh
chmod 600 ~/.ssh/authorized_keys
```

/etc/ssh/sshd_configを少し改変
```
# 不正アクセス防止のため、ポート番号をデフォルトの 22 から変更する。
Port 30000

# root ユーザでのログインは完全に禁止する。
PermitRootLogin no

# 安全性の低いパスワードに認証は禁止し、常に秘密鍵を使用する。
PasswordAuthentication no
```

設定変更を繁栄する
```
systemctl restart sshd
```

## ファイヤウォールの再設定
ファイヤウォールの設定ファイルをコピー
```
cp /etc/nftables.conf /etc/nftables.conf.default
```
/etc/nftables.confを以下の様に改変
```
flush ruleset
#LANアドレスのトップ。
define localnet = 192.168.1.0/24
#IPv4の口出し
table ip filter {
  set dos_counter {
    type ipv4_addr . inet_service
    flags dynamic
  }
  set block_targets {
    type ipv4_addr . inet_service
    flags timeout
  }

  #サーバーへのアクセスに関して口出し
  chain input {
    type filter hook input priority filter; policy drop;

    # セッション確立後のパケットは許可
    ct state established,related counter accept

    # おかしいやつの破棄
    ct state invalid counter drop

    # ループバックの許可
    iifname "lo" counter accept

    # SYN/ACKでNEWなパケットに対してRSTを送る
    tcp flags & (syn | ack) == syn | ack ct state new counter log prefix "SYN/ACK and NEW:" reject with tcp reset

    # NEWだがSYNが立っていないパケットを破棄
    tcp flags & (fin | syn | rst | ack) != syn ct state new counter log prefix "NEW not SYN:" drop

    # Lanないのやつipv4のみで。(今回は多分使われない。)
    ip saddr $localnet ct state new tcp dport . ip protocol {
      53 . tcp, 53 . udp,  # DNS
      123 . udp, # NTP
      137 . udp, 138 . udp, 139 . tcp,  # Windows
      445 . tcp,  # Active Dierctory
      30000 . tcp, # ssh
      17500 . tcp, 17500 . udp, # Dropbox (LAN sync)
    } counter accept

    tcp dport 30000 tcp flags & (fin | syn | rst | ack) == syn ct state new goto dos_detection

    #全てにおいて　
    ct state new tcp dport . ip protocol {
      80 . tcp, 443 . tcp,  # HTTP/HTTPS
      20 . tcp, 21 . tcp, #FTP/FTPS
			40000 . tcp, 40001 . tcp, 
			40002 . tcp, 40003 . tcp, 
			40004 . tcp, 40005 . tcp, 
			40006 . tcp, 40007 . tcp, 
			40008 . tcp, 40009 . tcp, 
			40010 . tcp, 40011 . tcp, 
			40012 . tcp, 40013 . tcp, 
			40014 . tcp, 40015 . tcp, 
			40016 . tcp, 40017 . tcp, 
			40018 . tcp, 40019 . tcp, 
			40020 . tcp, 40021 . tcp, 
			40022 . tcp, 40023 . tcp, 
			40024 . tcp, 40025 . tcp, 
			40026 . tcp, 40027 . tcp, 
			40028 . tcp, 40029 . tcp, 
			40030 . tcp, 40031 . tcp, 
			40032 . tcp, 40033 . tcp, #Ftp/Ftps for passive
    } counter accept

    # Ident(113)を拒否(DROPするとレスポンスが遅くなるのでReject)
    tcp dport 113 counter reject with tcp reset

    # ping飛ばすとき
    icmp type { echo-reply, echo-request } counter accept
  }
	#外だしに関して
  chain output {
    type filter hook output priority filter; policy accept;
    oifname "lo" counter accept
  }
  #対Dos
  chain dos_detection {
    counter
    add @dos_counter { ip saddr . tcp dport limit rate over 10/minute burst 5 packets } counter jump add_block
    ip saddr . tcp dport @block_targets counter drop
    counter accept
  }
  #対Dos
  chain add_block {
    # 攻撃を記録(最初だけ)
    ip saddr . tcp dport != @block_targets log prefix "DOS Attacked:"
    update @block_targets { ip saddr . tcp dport timeout 30m }
  }
}

#IPv6の拒絶
table ip6 filter {
	chain input {
		meta nfproto 6 drop
	}
}
```
このあと再起動

参考:https://knowledge.sakura.ad.jp/22636/#i-10

## FTPSサーバー作成
explicitモードで作成

FTPサーバーの起動とその確認
```
systemctl start vsftpd.service
systemctl status vsftpd.service
```
スタートアップアプリ化
```
systemctl enable vsftpd.service
```
/etc/vsftpd.confを以下のように変更
```
# local_enable=YES
# write_enable=YES
```
to
```
local_enable=YES
write_enable=YES
```
FTPリクエストを行い、ファイヤーウォールの確認
```
ftp localhost
```
できたら「/etc/vsftpd/vsftpd.conf」を編集
```
# パッシブモードも有効にする
pasv_enable=YES
# パッシブモードで使用するポート範囲設定
pasv_min_port=40000
pasv_max_port=40033
# 暗号化
ssl_enable=YES
# 暗号化プロトコルを設定
ssl_tlsv1=YES
ssl_sslv2=NO
ssl_sslv3=NO
# 暗号化していない通信を拒絶
force_local_data_ssl=YES
force_local_logins_ssl=YES
# 暗号化ファイル
rsa_cert_file=/etc/pki/tls/certs/vsftpd.pem
# 匿名ユーザーを拒絶
anonymous_enable=NO
anon_upload_enable=NO
#ログ関係の設定
xferlog_enable=YES
xferlog_std_format=YES
# 20番ポートからの接続を許可
connect_from_port_20=YES
# ユーザー毎にアクセスを変更
chroot_local_user=YES
chroot_list_enable=YES
chroot_list_file=/etc/vsftpd/chroot_list
# アクセス許可
userlist_enable=YES
userlist_deny=NO
userlist_file=/etc/vsftpd/user_list
# スタンドアロンモード(デーモン)で起動するかどうかを指定する
listen=YES
# GMTではなくローカル時間(=日本時間)に
use_localtime=YES
# アクセス制御
tcp_wrappers=YES
```
「fesadmin」、「fespublic」をユーザーに追加。<br>
どちらにもパスワードをかけておく。public のパスワードは共有しておくこと。

「/etc/vsftpd/chroot_list」を編集
```
fesadmin
```
「/etc/vsftpd/user_list」を編集
```
fesadmin
fespublic
```
最後にサービスの再起動
```
systemctl restart vsftpd
```