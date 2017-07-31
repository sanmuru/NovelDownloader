#pragma once

#ifndef _PLUGIN_H_
#define _PLUGIN_H_

struct PluginInterface_;
struct Plugin_;

#ifndef PLUGIN_INHERITANCE
#define PLUGIN_INHERITANCE
#ifdef __cplusplus
typedef struct Plugin_ Plugin;
#else
typedef const struct PluginInterface_ *Plugin;
#endif
#endif
#include "Plugin.Core.Win32.h"
#ifdef PLUGIN_INHERITANCE
#undef PLUGIN_INHERITANCE
#endif

static void CreateInstance_PluginInterface_(PluginInterface_ *p_pi);
struct PluginInterface_
{
	void *reserved_inner_plugin;

	LPCTSTR Name; // 插件的名称。
	LPCTSTR DisplayName; // 插件显示在插件管理器中的名称。
	::Version *Version; // 插件的版本。
	::Version *MinVersion; // 插件支持的最小版本。
	LPCTSTR Description; // 插件的描述。
	GUID *Guid; // 插件的全局唯一标识符。

#ifdef __cplusplus
	PluginInterface_()
	{
		size_t size = sizeof(PluginInterface_);
		PluginInterface_ *p_pi = NULL;
		::CreateInstance_PluginInterface_(p_pi);

		memcpy_s(this, size, p_pi, size);
	}
	~PluginInterface_() {}
#endif
};

static void CreateInstance_PluginInterface_(PluginInterface_ *p_pi)
{
	::PluginInterface_ pi = {};
	pi.reserved_inner_plugin = NULL;
	pi.Name = TEXT(EMPTY_STRING);
	pi.DisplayName = TEXT(EMPTY_STRING);
	*pi.Version = DEFAULT_VERSION;
	*pi.MinVersion = DEFAULT_VERSION;
	pi.Description = TEXT(EMPTY_STRING);
	*pi.Guid = DEFAULT_GUID;

	p_pi = &pi;
}

struct Plugin_
{
#ifdef __cplusplus
protected:
	const void *inner_plugin;

public:
	LPCTSTR Name; // 插件的名称。
	LPCTSTR DisplayName; // 插件显示在插件管理器中的名称。
	::Version *Version; // 插件的版本。
	::Version *MinVersion; // 插件支持的最小版本。
	LPCTSTR Description; // 插件的描述。
	GUID *Guid; // 插件的全局唯一标识符。

	Plugin_();
	~Plugin_();
#endif
};

#if defined __cplusplus & defined C_EXPORTS
EXTERN_C
{
#endif
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR Plugin_Name(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR Plugin_DisplayName(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API HVERSION Plugin_Version(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API HVERSION Plugin_MinVersion(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR Plugin_Description(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API GUID Plugin_Guid(HPLUGIN);
#if defined __cplusplus & defined C_EXPORTS
}
#endif
#endif