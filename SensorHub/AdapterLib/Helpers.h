//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include "pch.h"
#include <ppltasks.h>
using namespace concurrency;
using namespace Windows::Data::Json;

namespace  AdapterLib
    {
		class Helpers sealed
		{
		public:
			static concurrency::task<Platform::String ^> GetTextResultAsync(Windows::Web::Http::HttpResponseMessage^ response, 
				concurrency::cancellation_token token);
		//	static task<HttpResponseMessage^> GetJSONResultAsync(Platform::String^ output, Windows::Web::Http::HttpResponseMessage^ response);

			static Windows::Web::Http::HttpClient^ CreateHttpClient();

		/*	static Platform::String^ Trim(Platform::String^ s);

			static void ReplaceAll(std::wstring& value, _In_z_ const char16* from, _In_z_ const char16* to);

			static std::wstring::size_type IndexOf(Platform::String^ s, const wchar_t value);

			static Platform::String^ Substring(
				Platform::String^ s,
				std::wstring::size_type startIndex,
				std::wstring::size_type length);*/

		};
}

