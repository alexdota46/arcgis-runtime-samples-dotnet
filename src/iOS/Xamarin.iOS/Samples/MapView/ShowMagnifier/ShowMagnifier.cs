// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using System;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.UI;
using Foundation;
using UIKit;

namespace ArcGISRuntime.Samples.ShowMagnifier
{
    [Register("ShowMagnifier")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Show magnifier",
        "MapView",
        "This sample demonstrates how you can tap and hold on a map to get the magnifier. You can also pan while tapping and holding to move the magnifier across the map.",
        "")]
    public class ShowMagnifier : UIViewController
    {
        // Create and hold a reference to the used MapView.
        private readonly MapView _myMapView = new MapView();

        public ShowMagnifier()
        {
            Title = "Show magnifier";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create the UI, setup the control references and execute initialization.
            CreateLayout();
            Initialize();
        }

        public override void ViewDidLayoutSubviews()
        {
            nfloat topMargin = NavigationController.NavigationBar.Frame.Height + UIApplication.SharedApplication.StatusBarFrame.Height;

            // Reposition the control.
            _myMapView.Frame = new CoreGraphics.CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
            _myMapView.ViewInsets = new UIEdgeInsets(topMargin, 0, 0, 0);

            base.ViewDidLayoutSubviews();
        }

        private void Initialize()
        {
            // Create new Map with basemap and initial location.
            Map myMap = new Map(BasemapType.Topographic, 34.056295, -117.195800, 10);

            // Enable magnifier.
            _myMapView.InteractionOptions = new MapViewInteractionOptions
            {
                IsMagnifierEnabled = true
            };

            // Show the map in the view.
            _myMapView.Map = myMap;
        }

        private void CreateLayout()
        {
            // Add the map view to the view.
            View.AddSubviews(_myMapView);
        }
    }
}