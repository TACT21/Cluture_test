var editor;
var isInz = false;

function Inz(id, language, content) {
	var lang = insert_link("./script/Ace/src-min-noconflict/mode-" + language+".js");
	lang.onload = function (){
		Core(id, language, content);
    }
}

function Insert_link(target) {
	var script = document.createElement("script");
	script.setAttribute("src", target);
	document.getElementsByTagName("body")[0].appendChild(script);
	return script
}

function Core(id, language, content) {
	editor = ace.edit(id);
	editor.setTheme("ace/theme/tomorrow");
	editor.getSession().setMode("ace/mode/" + language);
	/*editor.keyBinding.*///Keybind
}

function Get_Value() {
	editor.getSession().getValue();
}

function Set_Value() {

}