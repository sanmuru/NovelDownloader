#pragma once

#include <Windows.h>

#define DECLARE_PLUGIN_HANDLE(type) typedef HANDLE H##type;
/// <summary>ָ��������ľ����</summary>
DECLARE_PLUGIN_HANDLE(PLUGIN)
/// <summary>ָ���鼮����ľ����</summary>
DECLARE_PLUGIN_HANDLE(BOOK)
/// <summary>ָ������ľ����</summary>
DECLARE_PLUGIN_HANDLE(VOLUME)
/// <summary>ָ���½ڶ���ľ����</summary>
DECLARE_PLUGIN_HANDLE(CHAPTER)

#define DECLARE_PLUGIN_ITERATOR(name, type) typedef void (*name##Iterator) (type);
/// <summary>ö�ٲ������Ļص�������</summary>
DECLARE_PLUGIN_ITERATOR(Plugin, HPLUGIN)
DECLARE_PLUGIN_ITERATOR(Host, LPWSTR)

/// <summary>��ȡ���в������</summary>
/// <param name="lpPattern">ɸѡ��������������ַƥ��ģʽ��</param>
/// <param name="interator">���ز������Ļص�������</param>
/// <returns>���ز�������������</returns>
int GetAllPlugins(LPWSTR lpPattern, PluginIterator iterator);
/// <summary>��ȡָ������������������������ַ��</summary>
/// <param name="interator">����������ַ�Ļص�������</param>
/// <returns>����������ַ��������</returns>
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