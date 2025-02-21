_____Maha's Slider System (SimpleScroll)______

A versatile, customizable slider widget for Unity that supports horizontal or vertical scrolling,
snapping, and optional navigation buttons. Perfect for carousels, tutorials, or scrollable menus!

______________________________________________

1. Overview
Maha's Slider System is a lightweight Unity package designed to simplify creating horizontal or vertical sliders with:

+Snapping between panels
+Centering elements if desired
+Easy navigation via buttons
+Clear, modular design (using SimpleScroll, ScrollArranger, ScrollSnapper, and optional SliderButtonManager)

2. Installation
+Import the .unitypackage:
 -> In Unity, go to Assets > Import Package > Custom Package...
 ->Select Maha_SliderSystem.unitypackage and click Import.
+(Optional) Open the Demo Scene (in Assets/MahaSliderPackage/Demo/) to see a working example.

3. Quick Start

+ create a new UI panel in your scene.
+ Attach Scripts:
 - SimpleScroll: Main coordinator for drag events and snapping.
 - ScrollArranger: Lays out UI elements horizontally/vertically.
 - ScrollSnapper: Calculates snap points and handles transitions.

+ Put Your UI Elements Under ScrollRect.content:
 - Each child element will be treated as a “slide”.
+ Configure Settings in the Inspector:
 - Scroll Direction (Horizontal or Vertical)
 - Snapping speed
 - Item Spacing
 - Center Elements (optional)

 4. Using Navigation Buttons

If you want previous/next buttons:

+ Add the SliderButtonManager script to a GameObject.
+ Assign your buttons in the Inspector.
+ Assign your SimpleScroll reference to _customSlider.
+ you can customize if you want them to be activity or interability of each button on and off at the borders

5. Scripts Overview
+ SimpleScroll
  - Coordinates drag events, snapping updates, and basic “go to next/previous slide” logic.
  - Fires OnSlideChangedObservable for other scripts to react to slide changes.
+ ScrollArranger
  - Arranges child elements horizontally or vertically with spacing and centering.
  - Sets the content.sizeDelta to match the total size.
+ ScrollSnapper
  - Calculates snap positions for each slide.
  - Finds the closest slide when dragging ends.
  - Provides methods for setting the scroll to a given slide.
+ SliderButtonManager (Optional)
  - Subscribes to SimpleScroll.OnSlideChangedObservable
  - Enables/disables Prev/Next buttons based on the current slide.
  - Requires references to SimpleScroll and two Buttons in the Inspector.

