// ExampleWin32Plugin.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "exports.h"
#include "ExampleWin32Plugin.h"

#define NOVELDOWNLOADERPLUGINCOREWIN32_EXPORTS
#include <malloc.h>
#include <Plugin.h>
#if defined __cplusplus and defined C_EXPORTS
EXTERN_C
{
#endif

EXAMPLEWIN32PLUGIN_API HPLUGIN LoadPlugin(GUID guid)
{
	if (true)
	{
		Plugin *plugin = {};
		plugin->Name = LPCTSTR("ExampleWin32Plugin");
		plugin->DisplayName = LPCTSTR("ExampleWin32Plugin");
		Version version = { 1,0,0,LPCTSTR("20161212"), LPCTSTR("base") };
		Version min_version = DEFAULT_VERSION;
		plugin->Version = &version;
		plugin->MinVersion = &min_version;
		plugin->Description = LPCTSTR("This is a example Win32 plugin.");
		plugin->Guid = &guid;

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

EXAMPLEWIN32PLUGIN_API GUID *GetPluginList()
{
	MALLOC(GUID, guid_array, sizeof(GUID) * 1)
	guid_array[0] = {1,1,1,{'1','1','1'}};
	return guid_array;
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

EXAMPLEWIN32PLUGIN_API Version Plugin_Version(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return *plugin->Version;
}

EXAMPLEWIN32PLUGIN_API Version Plugin_MinVersion(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return *plugin->MinVersion;
}

EXAMPLEWIN32PLUGIN_API LPCTSTR Plugin_Description(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return plugin->Description;
}

EXAMPLEWIN32PLUGIN_API GUID Plugin_Guid(HPLUGIN hPlugin){
	Plugin *plugin = FromHPLUGIN(hPlugin);
	return *plugin->Guid;
}

PSTRUCT_FROM_HANLDE(Version, HVERSION, hVersion)

EXAMPLEWIN32PLUGIN_API unsigned int Version_Major(HVERSION hVersion){
	Version *version = FromHVERSION(hVersion);
	return version->Major;
}

EXAMPLEWIN32PLUGIN_API unsigned int Version_Minor(HVERSION hVersion){
	Version *version = FromHVERSION(hVersion);
	return version->Minor;
}

EXAMPLEWIN32PLUGIN_API unsigned int Version_Revision(HVERSION hVersion){
	Version *version = FromHVERSION(hVersion);
	return version->Revision;
}

EXAMPLEWIN32PLUGIN_API LPCTSTR Version_Date(HVERSION hVersion){
	Version *version = FromHVERSION(hVersion);
	return version->Date;
}

EXAMPLEWIN32PLUGIN_API LPCTSTR Version_Period(HVERSION hVersion){
	Version *version = FromHVERSION(hVersion);
	return version->Period;
}

#if defined __cplusplus and defined C_EXPORTS
}
#endif