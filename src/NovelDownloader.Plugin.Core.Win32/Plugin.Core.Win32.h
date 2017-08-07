#pragma once

#ifndef _PLUGIN_CORE_WIN32_H_
#define _PLUGIN_CORE_WIN32_H_

#include "exports.h"
#include "macros.h"
#include <Windows.h>

#if defined __cplusplus & defined C_EXPORTS
EXTERN_C
{
#endif

typedef HANDLE HPLUGIN;

NOVELDOWNLOADERPLUGINCOREWIN32_API HPLUGIN APIENTRY LoadPlugin(GUID);
NOVELDOWNLOADERPLUGINCOREWIN32_API void APIENTRY ReleasePlugin(HPLUGIN);

NOVELDOWNLOADERPLUGINCOREWIN32_API HPLUGIN APIENTRY LoadPluginWithReleaseMethod(GUID, void(**)(HPLUGIN));

NOVELDOWNLOADERPLUGINCOREWIN32_API int APIENTRY GetPluginList(GUID**);

#if defined __cplusplus & defined C_EXPORTS
}
#endif
#endif