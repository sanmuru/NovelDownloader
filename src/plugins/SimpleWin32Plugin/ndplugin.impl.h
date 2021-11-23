#pragma once

#include <Windows.h>

struct Plugin;
struct Book;
struct Volume;
struct Chapter;

struct Plugin {
	LPWSTR* patterns;
	int lenPatterns;
	bool isSimple;
};

struct Book {
	LPWSTR Title;
	LPWSTR Author;
	LPWSTR* Tags;
	int lenTags;
	LPWSTR Description;

	Volume* Volumes;
	int lenVolumes;

	virtual void init() = 0;
};

struct Volume {
	LPWSTR Title;

	Chapter* Chapters;
	int lenChapters;
	Book* Book;

	virtual void init() = 0;
};

struct Chapter {
	LPWSTR Title;
	long long UpdatedTime;

	Volume* Volume;
	Book* Book;

	virtual void init() = 0;
};