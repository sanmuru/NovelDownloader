#pragma once
//#undef __cplusplus
#ifndef _NDTOKEN_H_
#define _NDTOKEN_H_

#include "exports.h"
#include "macros.h"
#include <Windows.h>
#include <list>

struct NDTokenInterface_;
struct NDToken_;

#ifdef __cplusplus
typedef struct NDToken_ NDToken;
#else
typedef struct NDTokenInterface_ *NDToken;
#endif

typedef struct
{
	LPCTSTR Message; // 参数的信息。
	DWORD Code; // 参数的错误码。
	void *Data; // 参数的其他数据。
} _Args;
typedef _Args Args;

typedef void(*EventHandler)(void*, void*);

struct NDTokenInterface_
{
	void *reserved_inner_token;

	NDToken *Parent; // 标志的父标志对象。
	NDToken *Children; // 标志的子标志对象集合。
	LPCTSTR Type; // 标志的类型。
	LPCTSTR Title; // 标志的标题。
	LPCTSTR Description; // 标志的描述。
	LPCTSTR Uri; // 标志的全局统一标识符。
	EventHandler *CreepStarted; // 当爬虫启动时调用的函数列表。
	EventHandler *CreepFetched; // 当爬虫捕捉到数据时调用的函数列表。
	EventHandler *CreepFinished; // 当爬虫终止时调用的函数列表。
	EventHandler *CreepErrored; // 当爬虫遇到内部错误时调用的函数列表。

	bool(*CanStartCreep)(NDToken*); // 检测是否可以启动爬虫。
	void(*StartCreep)(NDToken*); // 启动爬虫。
	void(*StartCreepInternal)(NDToken*); // 内部调用的启动爬虫的方法。

	bool(*CanCreep)(NDToken*, void*); // 检测爬虫是否可以爬向下一个标志。
	void*(*Creep)(NDToken*, void*); // 爬虫向下一个标志爬行。并获取采集到的数据。
	bool(*CreepInternal)(NDToken*); // 内部调用的爬虫爬行的方法。

	void(*OnCreepStarted)(NDToken*, void*, void*); // 引发CreepStarted的调用方法。
	void(*OnCreepFetched)(NDToken*, void*, void*); // 引发CreepFetched的调用方法。
	void(*OnCreepFinished)(NDToken*, void*, void*); // 引发CreepFinished的调用方法。
	void(*OnCreepErrored)(NDToken*, void*, void*); // 引发CreepErrored的调用方法。
};

struct NDToken_
{
#ifdef __cplusplus
protected:
	const void *inner_token;

public:
	NDToken *Parent; // 标志的父标志对象。
	NDToken *Children; // 标志的子标志对象集合。
	LPCTSTR Type; // 标志的类型。
	LPCTSTR Title; // 标志的标题。
	LPCTSTR Description; // 标志的描述。
	LPCTSTR Uri; // 标志的全局统一标识符。
	EventHandler *CreepStarted; // 当爬虫启动时调用的函数列表。
	EventHandler *CreepFetched; // 当爬虫捕捉到数据时调用的函数列表。
	EventHandler *CreepFinished; // 当爬虫终止时调用的函数列表。
	EventHandler *CreepErrored; // 当爬虫遇到内部错误时调用的函数列表。

	void *reserved_CanStopCreep;
	void *reserved_StartCreep;
	void *reserved_StartCreepInternal;
	
	void *reserved_CanCreep;
	void *reserved_Creep;
	void *reserved_CreepInternal;
	
	void *reserved_OnCreepStarted;
	void *reserved_OnCreepFetched;
	void *reserved_OnCreepFinished;
	void *reserved_OnCreepErrored;

	bool CanStartCreep(); // 检测是否可以启动爬虫。
	void StartCreep(); // 启动爬虫。
	void StartCreepInternal(); // 内部调用的启动爬虫的方法。

	bool CanCreep(void*); // 检测爬虫是否可以爬向下一个标志。
	template<typename TData> bool CanCreep(TData*); // 检测爬虫是否可以爬向下一个标志。
	void* Creep(void*); // 爬虫向下一个标志爬行。并获取采集到的数据。
	template<typename TData, typename TFetch> TFetch* Creep(TData*); // 爬虫向下一个标志爬行。并获取采集到的数据。
	bool CreepInternal(); // 内部调用的爬虫爬行的方法。

	void OnCreepStarted(void*, void*); // 引发CreepStarted的调用方法。
	void OnCreepFetched(void*, void*); // 引发CreepFetched的调用方法。
	template<typename TData> void OnCreepFetched(void*, TData*); // 引发CreepFetched的调用方法。
	void OnCreepFinished(void*, void*); // 引发CreepFinished的调用方法。
	void OnCreepErrored(void*, void*); // 引发CreepErrored的调用方法。
	template<typename TArgs> void OnCreepErrored(void*, TArgs*); // 引发CreepErrored的调用方法。
#endif
};

static int INTERNAL_NDToken_Children_count = 0;
static int INTERNAL_NDToken_Children_capacity = 0;
// 向标志的子标志对象集合中添加一个标志。
static void INTERNAL_Token_Children_add(NDTokenInterface_ *DestinateToken, NDToken *Token)
{
	if (DestinateToken == NULL || Token == NULL) return;

	if (SYNTAX_FILTER_ACCESS(Token)->Parent != NULL && POINTER_ADDRESS_EQUALS(SYNTAX_FILTER_ACCESS(Token)->Parent, DestinateToken))
	{
		Args args = { LPCTSTR("标志已经有父元素，无法覆盖。"), 0, NULL };
		throw args;
	}
	
	*SYNTAX_FILTER_ACCESS(Token)->Parent = *(NDToken*)DestinateToken; // 设置添加的标志的父元素。
	
	if (INTERNAL_NDToken_Children_count >= INTERNAL_NDToken_Children_capacity)
	{
		size_t size = sizeof(NDToken);
		MALLOC(NDToken, children, size * (INTERNAL_NDToken_Children_count + 10))
		if (DestinateToken->Children != NULL)
		{
			memcpy_s(children, size * INTERNAL_NDToken_Children_count, DestinateToken->Children, size * INTERNAL_NDToken_Children_count);
			FREE(NDToken, DestinateToken->Children)
		}
		DestinateToken->Children = children;
		FREE(NDToken, children);
		INTERNAL_NDToken_Children_capacity = INTERNAL_NDToken_Children_count + 10;
	}
	DestinateToken->Children[INTERNAL_NDToken_Children_count] = *Token;
	INTERNAL_NDToken_Children_count++;
}

// 从标志的子标志对象集合中移除一个标志。
static void INTERNAL_Token_Children_remove(NDTokenInterface_ *DestinateToken, NDToken *Token)
{
	if (DestinateToken == NULL || Token == NULL) return;

	for (int i = 0; i < INTERNAL_NDToken_Children_count; i++)
	{
		if (POINTER_ADDRESS_EQUALS(&(DestinateToken->Children[i]), Token))
		{
			size_t size = sizeof(NDToken) * (INTERNAL_NDToken_Children_count - i - 1);
			MALLOC(NDToken, temp, size)
			memcpy_s(&temp[0], size, &(DestinateToken->Children[i + 1]), size);
			memcpy_s(&(DestinateToken->Children[i]), size, &temp[0], size);
			FREE(NDToken, temp);
			INTERNAL_NDToken_Children_count--;
		}
	}
}

static int INTERNAL_Token_Children_indexof(NDTokenInterface_ *DestinateToken, NDToken *Token)
{
	if (DestinateToken == NULL || Token == NULL) return -1;

	for (int i = 0; i < INTERNAL_NDToken_Children_count; i++)
	{
		if (POINTER_ADDRESS_EQUALS(&(DestinateToken->Children[i]), Token))
			return i;
	}

	return -1;
}

static bool INTERNAL_Token_Children_contains(NDTokenInterface_ *DestinateToken, NDToken *Token)
{
	return INTERNAL_Token_Children_indexof(DestinateToken, Token) != -1;
}



#define EVENT_HANDLER_COLLECTION(struct_name, event_name, destinate_type, destinate_param, handler_type, handler_param)\
COLLECTION_FIELDS(struct_name, event_name)\
/* 向函数列表中添加一个函数。 */\
COLLECTION_ADD(struct_name, event_name, destinate_type, destinate_param, handler_type, handler_param, attach)\
/* 从函数列表中移除一个函数。 */\
COLLECTION_REMOVE(struct_name, event_name, destinate_type, destinate_param, handler_type, handler_param, remove)

EVENT_HANDLER_COLLECTION(NDTokenInterface_, CreepStarted, NDTokenInterface_, token, EventHandler, handler)
EVENT_HANDLER_COLLECTION(NDTokenInterface_, CreepFetched, NDTokenInterface_, token, EventHandler, handler)
EVENT_HANDLER_COLLECTION(NDTokenInterface_, CreepFinished, NDTokenInterface_, token, EventHandler, handler)
EVENT_HANDLER_COLLECTION(NDTokenInterface_, CreepErrored, NDTokenInterface_, token, EventHandler, handler)

#define EVENT_INVOKE_FUNC(struct_name, event_name, destinate_type, destinate_param)\
static void INTERNAL_##struct_name##_On##event_name##(destinate_type *destinate_param, void *sender, void *args)\
{\
	if (destinate_param == NULL) return;\
\
	for (int i = 0; i < INTERNAL_##struct_name##_##event_name##_count; i++)\
	{\
		destinate_param->##event_name##[i](sender, args);\
	}\
}

EVENT_INVOKE_FUNC(NDTokenInterface_, CreepStarted, NDTokenInterface_, token)
EVENT_INVOKE_FUNC(NDTokenInterface_, CreepFetched, NDTokenInterface_, token)
EVENT_INVOKE_FUNC(NDTokenInterface_, CreepFinished, NDTokenInterface_, token)
EVENT_INVOKE_FUNC(NDTokenInterface_, CreepErrored, NDTokenInterface_, token)

static void i()
{
#ifndef __cplusplus
	NDToken token = {};
	NDToken *p_token = &token;
	(*p_token)->Uri = LPCTSTR("");
	INTERNAL_NDTokenInterface__CreepErrored_attach((NDTokenInterface_*)&token, (EventHandler)NULL);
#endif
}


#ifdef __cplusplus
EXTERN_C
{
#endif
typedef HANDLE HNDTOKEN;

NOVELDOWNLOADERPLUGINCOREWIN32_API HNDTOKEN NDToken_Parent(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API HNDTOKEN *NDToken_Children(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR NDToken_Type(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR NDToken_Title(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR NDToken_Description(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API LPCTSTR NDToken_Uri(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API EventHandler *NDToken_CreepStarted(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API EventHandler *NDToken_CreepFetched(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API EventHandler *NDToken_CreepFinished(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API EventHandler *NDToken_CreepErrored(HNDTOKEN);

NOVELDOWNLOADERPLUGINCOREWIN32_API bool NDToken_CanStartCreep(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API void NDToken_StartCreep(HNDTOKEN);
//NOVELDOWNLOADERPLUGINCOREWIN32_API void NDToken_StartCreepInternal(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API bool NDToken_CanCreep(HNDTOKEN, void*);
NOVELDOWNLOADERPLUGINCOREWIN32_API void NDToken_Creep(HNDTOKEN, void*);
//NOVELDOWNLOADERPLUGINCOREWIN32_API bool NDToken_CreepInternal(HNDTOKEN);
NOVELDOWNLOADERPLUGINCOREWIN32_API void NDToken_OnCreepStarted(HNDTOKEN, void*, void*);
NOVELDOWNLOADERPLUGINCOREWIN32_API void NDToken_OnCreepFetched(HNDTOKEN, void*, void*);
NOVELDOWNLOADERPLUGINCOREWIN32_API void NDToken_OnCreepFinished(HNDTOKEN, void*, void*);
NOVELDOWNLOADERPLUGINCOREWIN32_API void NDToken_OnCreepErrored(HNDTOKEN, void*, void*);
#ifdef __cplusplus
}
#endif

#endif