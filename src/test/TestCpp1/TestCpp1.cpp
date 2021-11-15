#include <iostream>
#include "ndplugin.h"
#include "ndplugin.structure.h"

void piterator(HPLUGIN hPlugin)
{
    Plugin* plugin = (Plugin*)hPlugin;
    std::wcout << plugin->pattern;
    std::wcout << "\n";
}

int main()
{
    std::cout << "Hello World!\n";

    HMODULE hModule = LoadLibrary(L"TestCppPlugin.dll");
    if (hModule == nullptr) std::cout << "nullptr";

    typedef int (*GetAllPlugins) (LPWSTR lpHost, PluginIterator iterator);
    GetAllPlugins fGetAllPlugins = (GetAllPlugins)GetProcAddress(hModule, "GetAllPlugins");
}