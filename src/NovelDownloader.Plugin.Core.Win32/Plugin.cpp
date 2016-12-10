#include "stdafx.h"
#include "Plugin.h"

#ifdef __cplusplus

Plugin_::Plugin_()
{
	this->inner_plugin = this; // 内部的指针指向自身，以便逻辑基类调用子类的成员。
	this->Name = LPCTSTR(EMPTY_STRING);
	this->DisplayName = LPCTSTR(EMPTY_STRING);
	*this->Version = DEFAULT_VERSION;
	*this->MinVersion = DEFAULT_VERSION;
	this->Description = LPCTSTR(EMPTY_STRING);
	*this->Guid = DEFAULT_GUID;
}

Plugin_::~Plugin_()
{
	if (this->inner_plugin != NULL)
	{
		this->inner_plugin = NULL; // 最后将指向自身的内部指针指向空。
	}
}
#endif