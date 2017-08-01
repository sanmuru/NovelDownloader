// ExampleWin32Plugin.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "exports.h"
#include "ExampleWin32Plugin.h"

#ifdef _DEBUG
#include <stdio.h>
#endif

#define NOVELDOWNLOADERPLUGINCOREWIN32_EXPORTS
#include <malloc.h>
#include <Plugin.h>
#if defined __cplusplus & defined C_EXPORTS
EXTERN_C
{
#endif

EXAMPLEWIN32PLUGIN_API HPLUGIN LoadPlugin(GUID guid)
{
	if (true)
	{
		MALLOC(Plugin, plugin, sizeof(Plugin))
		plugin->Name = L"ExampleWin32Plugin";
		plugin->DisplayName = L"ExampleWin32Plugin";
		VERSION version = { 1,0,0,L"20161212", L"base" };
		VERSION min_version = DEFAULT_VERSION;
		plugin->Version = &version;
		plugin->MinVersion = &min_version;
		plugin->Description = L"This is an example Win32 plugin. 这是一个Win32插件实例。";
		plugin->Guid = &guid;

#ifdef _DEBUG
		wprintf_s(L"Name : %s\n", plugin->Name);
		wprintf_s(L"DisplayName : %s\n", plugin->DisplayName);
		wprintf_s(L"Version : V%d.%d.%d_%s_%s", 
			plugin->Version->Major,
			plugin->Version->Minor,
			plugin->Version->Revision,
			plugin->Version->Date,
			plugin->Version->Period
		);
		wprintf_s(L"MinVersion : V%d.%d.%d\n", 
			plugin->MinVersion->Major,
			plugin->MinVersion->Minor,
			plugin->MinVersion->Revision
		);
		wprintf_s(L"Description : %s\n", plugin->Description);
		wprintf_s(L"Guid : {%x-%x-%x-%x%x-%x%x%x%x%x%x}\n", 
			plugin->Guid->Data1, 
			plugin->Guid->Data2, 
			plugin->Guid->Data3, 
			plugin->Guid->Data4[1], 
			plugin->Guid->Data4[2], 
			plugin->Guid->Data4[3], 
			plugin->Guid->Data4[4], 
			plugin->Guid->Data4[5], 
			plugin->Guid->Data4[6], 
			plugin->Guid->Data4[7], 
			plugin->Guid->Data4[8]
		);
#endif

		return (HPLUGIN)plugin;
	}
}

#define PSTRUCT_FROM_HANLDE(struct_type, handle_type, handle_param)\
static struct_type *From##handle_type##(handle_type handle_param)\
{\
	return (struct_type*)handle_param;\
}

PSTRUCT_FROM_HANLDE(Plugin, HPLUGIN, hPlugin)

EXAMPLEWIN32PLUGIN_API void ReleasePlugin(HPLUGIN hPlugin)
{
	Plugin *plugin = FromHPLUGIN(hPlugin);
	plugin = NULL;
}

#include <objbase.h>
EXAMPLEWIN32PLUGIN_API int GetPluginList(GUID** plugins)
{
	int count = 1;
	GUID *guid_array = new GUID[1];

	CoCreateGuid(&guid_array[0]);
	
	*plugins = guid_array;

	return count;
}

EXAMPLEWIN32PLUGIN_API LPCTSTR Plugin_Name(HPLUGIN hPlugin)
{
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return plugin->Name;
}

EXAMPLEWIN32PLUGIN_API LPCTSTR Plugin_DisplayName(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return plugin->DisplayName;
}

EXAMPLEWIN32PLUGIN_API HVERSION Plugin_Version(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return plugin->Version;
}

EXAMPLEWIN32PLUGIN_API HVERSION Plugin_MinVersion(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return plugin->MinVersion;
}

EXAMPLEWIN32PLUGIN_API LPCTSTR Plugin_Description(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return plugin->Description;
}

EXAMPLEWIN32PLUGIN_API GUID Plugin_Guid(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return *plugin->Guid;
}

PSTRUCT_FROM_HANLDE(VERSION, HVERSION, hVersion)

EXAMPLEWIN32PLUGIN_API unsigned int Version_Major(HVERSION hVersion){
	VERSION *version = FromHVERSION(hVersion);
	return version->Major;
}

EXAMPLEWIN32PLUGIN_API unsigned int Version_Minor(HVERSION hVersion){
	VERSION *version = FromHVERSION(hVersion);
	return version->Minor;
}

EXAMPLEWIN32PLUGIN_API unsigned int Version_Revision(HVERSION hVersion){
	VERSION *version = FromHVERSION(hVersion);
	return version->Revision;
}

EXAMPLEWIN32PLUGIN_API LPCTSTR Version_Date(HVERSION hVersion){
	VERSION *version = FromHVERSION(hVersion);
	return version->Date;
}

EXAMPLEWIN32PLUGIN_API LPCTSTR Version_Period(HVERSION hVersion){
	VERSION *version = FromHVERSION(hVersion);
	return version->Period;
}

#if defined __cplusplus & defined C_EXPORTS
}
#endif