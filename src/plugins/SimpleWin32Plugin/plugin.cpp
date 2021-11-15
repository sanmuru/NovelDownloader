#include "ndplugin.h"
#include "ndplugin.structure.h"

int GetAllPlugins(LPWSTR lpPattern, PluginIterator iterator) {
	if (iterator == nullptr) throw "argument is nullptr: iterator";

	Plugin* pSimple = new Plugin();
	Plugin* pComplete = new Plugin();

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