#include "pch.h"
#include "ndplugin.h"
#include "ndplugin.impl.h"

int GetAllPlugins(LPWSTR lpPattern, PluginIterator iterator) {
	if (iterator == nullptr) throw "argument is nullptr: iterator";

	Plugin* pSimple = new Plugin(); pSimple->isSimple = true;
	Plugin* pComplete = new Plugin(); pSimple->isSimple = false;

	iterator(pSimple); // return simple plugin.
	if (lpPattern != nullptr) iterator(pComplete); // return complete plugin.

	return 2;
}

int GetCompatibleHosts(HPLUGIN hPlugin, HostIterator iterator) {
	if (hPlugin == nullptr) throw "argument is nullptr: hPlugin";
	if (iterator == nullptr) throw "argument is nullptr: iterator";

	Plugin* plugin = (Plugin*)hPlugin;
	for (int i = 0; i < plugin->lenPatterns; i++) {
		iterator(plugin->patterns[i]);
	}

	return plugin->lenPatterns;
}

HBOOK Activate(HPLUGIN hPlugin, LPWSTR lpUri) {
	if (hPlugin == nullptr) throw "argument is nullptr: hPlugin";
	
	Plugin* plugin = (Plugin*)hPlugin;
	if (plugin->isSimple) {
		// simple plugin.
		// 简单插件只提供固定的数据和信息。
	}
	else {
		// complete plugin.
		// 完整插件迭代遍历一个表示书籍的本机目录，从中获取信息。

	}
}