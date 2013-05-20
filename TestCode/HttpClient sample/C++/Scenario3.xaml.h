//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "pch.h"
#include "Scenario3.g.h"
#include "MainPage.xaml.h"
#include "HttpRequest.h"

namespace SDKSample
{
    namespace HttpClientSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [ Windows::Foundation::Metadata::WebHostHidden ]
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
 
        private:
            MainPage^ rootPage;
            Web::HttpRequest httpRequest;
            concurrency::cancellation_token_source cancellationTokenSource;
            void Start_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Cancel_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DisplayTextResult(const std::wstring& response, Windows::UI::Xaml::Controls::TextBox^ output);
        };
    }
}