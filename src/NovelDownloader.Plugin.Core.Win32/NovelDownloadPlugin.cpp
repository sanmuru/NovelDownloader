#include "stdafx.h"
#include "NovelDownloadPlugin.h"

#ifdef __cplusplus

static bool TryGetBookTokenInternal(Plugin *hPlugin, LPCTSTR uri, NDTBook *bookToken)
{
	return false;
}

bool NovelDownloadPlugin_::TryGetBookToken(LPCTSTR uri, NDTBook* bookToken)
{
	return ((NovelDownloadPluginInterface_*)this->inner_plugin)->TryGetBookToken(this, uri, bookToken);
}

NovelDownloadPlugin_::NovelDownloadPlugin_()
{
	this->inner_plugin = this; // 内部的指针指向自身，以便逻辑基类调用子类的成员。
	((NovelDownloadPluginInterface_*)this)->TryGetBookToken = &TryGetBookTokenInternal;
}

NovelDownloadPlugin_::~NovelDownloadPlugin_()
{
	if (this->inner_plugin != NULL)
	{
		this->inner_plugin = NULL; // 最后将指向自身的内部指针指向空。
	}
}
#endif