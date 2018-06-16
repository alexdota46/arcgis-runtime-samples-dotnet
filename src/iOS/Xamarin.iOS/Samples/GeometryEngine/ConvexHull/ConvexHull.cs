// Copyright 2018 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using System;
using UIKit;

namespace ArcGISRuntime.Samples.ConvexHull
{
    [Register("ConvexHull")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Convex hull",
        "GeometryEngine",
        "This sample demonstrates how to use the GeometryEngine.ConvexHull operation to generate a polygon that encloses a series of user-tapped map points.",
        "Tap on the map in several places, then click the 'Convex Hull' button.",
        "Analysis", "ConvexHull", "GeometryEngine")]
    public class ConvexHull : UIViewController
    {
        // Create and hold a reference to the used MapView.
        private readonly MapView _myMapView = new MapView();

        // Graphics overlay to display the graphics.
        private GraphicsOverlay _graphicsOverlay;

        // List of geometry values (MapPoints in this case) that will be used by the GeometryEngine.ConvexHull operation.
        private readonly PointCollection _inputPointCollection = new PointCollection(SpatialReferences.WebMercator);

        // Text view to display the instructions on how to use the sample.
        private UILabel _helpLabel;

        // Create a UIButton to create a convex hull.
        private UIButton _convexHullButton;

        // Create toolbars to put behind the label and button.
        private readonly UIToolbar _helpToolbar = new UIToolbar();
        private readonly UIToolbar _controlsToolbar = new UIToolbar();

        public ConvexHull()
        {
            Title = "Convex hull";
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
            nfloat topStart = NavigationController.NavigationBar.Frame.Height + UIApplication.SharedApplication.StatusBarFrame.Height;
            nfloat controlHeight = 30;
            nfloat margin = 5;

            // Setup the visual frames for the controls.
            _myMapView.Frame = new CoreGraphics.CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
            _helpToolbar.Frame = new CoreGraphics.CGRect(0, topStart, View.Bounds.Width, controlHeight + 2 * margin);
            _controlsToolbar.Frame = new CoreGraphics.CGRect(0, View.Bounds.Height - controlHeight - 2 * margin, View.Bounds.Width, controlHeight + 2 * margin);
            _helpLabel.Frame = new CoreGraphics.CGRect(margin, topStart + margin, View.Bounds.Width - 2 * margin, controlHeight);
            _convexHullButton.Frame = new CoreGraphics.CGRect(margin, View.Bounds.Height - margin - controlHeight, View.Bounds.Width - 2 * margin, controlHeight);

            base.ViewDidLayoutSubviews();
        }

        private void Initialize()
        {
            // Create a map with a topographic basemap.
            Map theMap = new Map(Basemap.CreateTopographic());

            // Assign the map to the MapView.
            _myMapView.Map = theMap;

            // Create a graphics overlay to hold the various graphics.
            _graphicsOverlay = new GraphicsOverlay();

            // Add the created graphics overlay to the MapView.
            _myMapView.GraphicsOverlays.Add(_graphicsOverlay);

            // Wire up the MapView's GeoViewTapped event handler.
            _myMapView.GeoViewTapped += MyMapView_GeoViewTapped;
        }

        private void MyMapView_GeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                // Create a map point (in the WebMercator projected coordinate system) from the GUI screen coordinate.
                MapPoint userTappedMapPoint = _myMapView.ScreenToLocation(e.Position);

                // Add the map point to the list that will be used by the GeometryEngine.ConvexHull operation.
                _inputPointCollection.Add(userTappedMapPoint);

                // Create a simple marker symbol to display where the user tapped/clicked on the map. The marker symbol 
                // will be a solid, red circle.
                SimpleMarkerSymbol userTappedSimpleMarkerSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle,
                    System.Drawing.Color.Red, 10);

                // Create a new graphic for the spot where the user clicked on the map using the simple marker symbol. 
                Graphic userTappedGraphic = new Graphic(userTappedMapPoint, userTappedSimpleMarkerSymbol)
                {
                    // Set the Z index for the user tapped graphic so that it appears above the convex hull graphic(s) added later.
                    ZIndex = 1
                };

                // Add the user tapped/clicked map point graphic to the graphic overlay.
                _graphicsOverlay.Graphics.Add(userTappedGraphic);
            }
            catch (System.Exception ex)
            {
                // Display an error message if there is a problem adding user tapped graphics.
                UIAlertController alertController = UIAlertController.Create("Can't add user tapped graphic", ex.Message, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(alertController, true, null);
            }
        }

        private void ConvexHullButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a multi-point geometry from the user tapped input map points.
                Multipoint inputMultipoint = new Multipoint(_inputPointCollection);

                // Get the returned result from the convex hull operation.
                Geometry convexHullGeometry = GeometryEngine.ConvexHull(inputMultipoint);

                // Create a simple line symbol for the outline of the convex hull graphic(s).
                SimpleLineSymbol convexHullSimpleLineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid,
                    System.Drawing.Color.Blue, 4);

                // Create the simple fill symbol for the convex hull graphic(s) - comprised of a fill style, fill 
                // color and outline. It will be a hollow (i.e.. see-through) polygon graphic with a thick red outline.
                SimpleFillSymbol convexHullSimpleFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Null,
                    System.Drawing.Color.Red, convexHullSimpleLineSymbol);

                // Create the graphic for the convex hull - comprised of a polygon shape and fill symbol.
                Graphic convexHullGraphic = new Graphic(convexHullGeometry, convexHullSimpleFillSymbol)
                {
                    ZIndex = 0
                };

                // Set the Z index for the convex hull graphic so that it appears below the initial input user 
                // tapped map point graphics added earlier.

                // Add the convex hull graphic to the graphics overlay collection.
                _graphicsOverlay.Graphics.Add(convexHullGraphic);

                // Disable the button after has been used.
                _convexHullButton.Enabled = false;
                _convexHullButton.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            }
            catch (System.Exception ex)
            {
                // Display an error message if there is a problem generating convex hull operation.
                UIAlertController alertController = UIAlertController.Create("Geometry Engine Failed!", ex.Message, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(alertController, true, null);
            }
        }

        private void CreateLayout()
        {
            // Create a UITextView for the overall sample instructions.
            _helpLabel = new UILabel
            {
                Text = "Tap on the map in several places, then tap 'Create convex hull'.",
                AdjustsFontSizeToFitWidth = true,
                Lines = 1
            };

            // Create a UIButton to create the convex hull.
            _convexHullButton = new UIButton();
            _convexHullButton.SetTitle("Create convex hull", UIControlState.Normal);
            _convexHullButton.SetTitleColor(View.TintColor, UIControlState.Normal);

            // - Hook to touch event to do querying
            _convexHullButton.TouchUpInside += ConvexHullButton_Click;

            // Add the MapView and other controls to the page.
            View.AddSubviews(_myMapView, _helpToolbar, _controlsToolbar, _helpLabel, _convexHullButton);
        }
    }
}