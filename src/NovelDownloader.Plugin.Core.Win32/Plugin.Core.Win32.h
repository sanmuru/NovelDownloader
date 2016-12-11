#pragma once

#ifndef _PLUGIN_CORE_WIN32_H_
#define _PLUGIN_CORE_WIN32_H_

#include "exports.h"
#include "macros.h"
#include "Version.h"
#include <Windows.h>

#ifdef __cplusplus
EXTERN_C
{
#endif

typedef HANDLE HPLUGIN;

NOVELDOWNLOADERPLUGINCOREWIN32_API HPLUGIN LoadPlugin(GUID);
NOVELDOWNLOADERPLUGINCOREWIN32_API void ReleasePlugin(HPLUGIN);
NOVELDOWNLOADERPLUGINCOREWIN32_API GUID *GetPluginList();

#ifdef __cplusplus
}
#endif
#endif