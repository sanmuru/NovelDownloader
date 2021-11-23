#pragma once

#include <Windows.h>

#define DECLARE_PLUGIN_HANDLE(type) typedef HANDLE H##type;
/// <summary>指向插件对象的句柄。</summary>
DECLARE_PLUGIN_HANDLE(PLUGIN)
/// <summary>指向书籍对象的句柄。</summary>
DECLARE_PLUGIN_HANDLE(BOOK)
/// <summary>指向卷对象的句柄。</summary>
DECLARE_PLUGIN_HANDLE(VOLUME)
/// <summary>指向章节对象的句柄。</summary>
DECLARE_PLUGIN_HANDLE(CHAPTER)

#define DECLARE_PLUGIN_ITERATOR(name, type) typedef void (*name##Iterator) (type);
/// <summary>枚举插件对象的回调函数。</summary>
DECLARE_PLUGIN_ITERATOR(Plugin, HPLUGIN)
DECLARE_PLUGIN_ITERATOR(Host, LPWSTR)

/// <summary>获取所有插件对象。</summary>
/// <param name="lpPattern">筛选插件对象的主机地址匹配模式。</param>
/// <param name="interator">返回插件对象的回调函数。</param>
/// <returns>返回插件对象的数量。</returns>
int GetAllPlugins(LPWSTR lpPattern, PluginIterator iterator);
/// <summary>获取指定插件对象适配的所有主机地址。</summary>
/// <param name="interator">返回主机地址的回调函数。</param>
/// <returns>返回主机地址的数量。</returns>
int GetCompatibleHosts(HPLUGIN hPlugin, HostIterator iterator);

HBOOK Activate(HPLUGIN hPlugin, LPWSTR lpUri);
BOOL Deactivate(HBOOK hBook, BOOL recurse);

LPWSTR Book_GetTitle(HBOOK hBook);
LPWSTR Book_GetAuthor(HBOOK hBook);
DECLARE_PLUGIN_ITERATOR(Tag, LPWSTR)
int Book_GetTags(HBOOK hBook, TagIterator iterator);
LPWSTR Book_GetDescription(HBOOK hBook);
DECLARE_PLUGIN_ITERATOR(Volume, HVOLUME)
int Book_GetVolumes(HBOOK hBook, VolumeIterator iterator);

LPWSTR Volume_GetTitle(HVOLUME hVolume);
DECLARE_PLUGIN_ITERATOR(Chapter, HCHAPTER)
int Volume_GetChapters(HVOLUME hVolume, ChapterIterator iterator);

LPWSTR Chapter_GetTitle(HCHAPTER hChapter);
long long Chapter_GetUpdatedAt(HCHAPTER hChapter);