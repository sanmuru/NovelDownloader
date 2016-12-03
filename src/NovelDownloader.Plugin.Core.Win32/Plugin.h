#pragma once

#ifndef _PLUGIN_H_
#define _PLUGIN_H_
#include "Plugin.Core.Win32.h"

struct PluginInterface_
{
	void *reserved_inner_plugin;

	LPCTSTR Name; // 插件的名称。
	LPCTSTR DisplayName; // 插件显示在插件管理器中的名称。
	::Version Version; // 插件的版本。
	::Version MinVersion; // 插件支持的最小版本。
	LPCTSTR Description; // 插件的描述。
};

struct Plugin_
{
#ifdef __cplusplus
protected:
	const void *inner_plugin;

public:
	LPCTSTR Name; // 插件的名称。
	LPCTSTR DisplayName; // 插件显示在插件管理器中的名称。
	::Version Version; // 插件的版本。
	::Version MinVersion; // 插件支持的最小版本。
	LPCTSTR Description; // 插件的描述。

	Plugin_();
	~Plugin_();
#endif
};

#ifdef __cplusplus
EXTERN_C
{
#endif
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR Plugin_Name(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR Plugin_DisplayName(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API Version Plugin_Version(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API Version Plugin_MinVersion(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR Plugin_Description(HPLUGIN);
#ifdef __cplusplus
}
#endif
#endif