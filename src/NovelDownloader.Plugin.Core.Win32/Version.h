#pragma once

#ifndef _VERSION_H_
#define _VERSION_H_

#include "exports.h"
#include <Windows.h>

typedef struct
{
	unsigned int Major; // 主版本号
	unsigned int Minor; // 次版本号
	unsigned int Revision; // 阶段版本号
	LPCTSTR Date; // 日期版本号
	LPCTSTR Period; // 希腊字母版本号
} _Version;
typedef _Version *Version;

#ifdef __cplusplus
EXTERN_C
{
#endif
typedef HANDLE HVERSION; // 版本号结构的句柄。

NOVELDOWNLOADERPLUGINCOREWIN32_API unsigned int Version_Major(HVERSION);
NOVELDOWNLOADERPLUGINCOREWIN32_API unsigned int Version_Minor(HVERSION);
NOVELDOWNLOADERPLUGINCOREWIN32_API unsigned int Version_Revision(HVERSION);

NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR Version_Date(HVERSION);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR Version_Period(HVERSION);
#ifdef __cplusplus
}
#endif

#endif