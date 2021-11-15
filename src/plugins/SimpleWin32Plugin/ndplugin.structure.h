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
};

struct Volume {
	LPWSTR Title;

	Chapter* Chapters;
	int lenChapters;
	Book* Book;
};

struct Chapter {
	LPWSTR Title;
	long long UpdatedTime;

	Volume* Volume;
	Book* Book;
};