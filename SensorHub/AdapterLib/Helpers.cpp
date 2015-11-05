//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "Helpers.h"
#include <ppltasks.h>

using namespace concurrency;
using namespace Windows::Web;
using namespace Windows::Web::Http;
using namespace Windows::Web::Http::Filters;
using namespace Windows::Web::Http::Headers;

using namespace Windows::Data::Json;
using namespace Windows::Foundation;
using namespace Windows::Security::Cryptography;

using namespace Windows::Storage::Streams;

namespace  AdapterLib
{
	task<HttpResponseMessage^> Helpers::GetJSONResultAsync(Platform::String ^ output, Windows::Web::Http::HttpResponseMessage ^ response)
	{

		// Read content as string. We need to use use_current() with the continuations since the tasks are completed on
		// background threads and we need parse JSON.
		task<Platform::String^> readAsStringTask(response->Content->ReadAsStringAsync());

		return readAsStringTask.then([=](Platform::String^ responseBodyAsText) {
			
			//output += responseBodyAsText;

			return response;

		}, task_continuation_context::use_current());

		/*task<Platform::String^> theTask = create_task(response->Content->ReadAsStringAsync()).then([=](Platform::String^ responseBodyAsText) {
			return responseBodyAsText;
		}, task_continuation_context::use_current());

		return theTask;*/
	}


	HttpClient^ Helpers::CreateHttpClient()
	{
		HttpClient^ httpClient = ref new HttpClient();

		Platform::String^ username = "Administrator";
		Platform::String^ password = "p@ssw0rd";
		IBuffer^ buffer = CryptographicBuffer::ConvertStringToBinary(username + L":" + password, BinaryStringEncoding::Utf16LE);
		Platform::String^ base64token = CryptographicBuffer::EncodeToBase64String(buffer);

		httpClient->DefaultRequestHeaders->Authorization = ref new HttpCredentialsHeaderValue(L"Basic", base64token);

		return httpClient;
	}

	/*

	Platform::String^ Helpers::Trim(Platform::String^ s)
	{
		const char16* first = s->Begin();
		const char16* last = s->End();

		while (first != last && iswspace(*first))
		{
			++first;
		}

		while (first != last && iswspace(last[-1]))
		{
			--last;
		}

		return ref new Platform::String(first, (int)(last - first));
	}

	void Helpers::ReplaceAll(
		std::wstring& value,
		_In_z_ const char16* from,
		_In_z_ const char16* to)
	{
		auto fromLength = wcslen(from);
		std::wstring::size_type offset;

		for (offset = value.find(from); offset != std::wstring::npos; offset = value.find(from, offset))
		{
			value.replace(offset, fromLength, to);
		}
	}

	std::wstring::size_type Helpers::IndexOf(Platform::String^ s, const wchar_t value)
	{
		std::wstring sourceString(s->Data());
		return sourceString.find(value);
	}

	Platform::String^ Helpers::Substring(Platform::String^ s, std::wstring::size_type startIndex, std::wstring::size_type length)
	{
		std::wstring sourceString(s->Data());
		std::wstring substring = sourceString.substr(startIndex, length);
		return ref new Platform::String(substring.data());
	}*/
}