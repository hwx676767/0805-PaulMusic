GlitchLab Pro – Analog & Digital Post FX v1.0 — README

THANK YOU
Thank you for your purchase.

ABOUT
Glitch Lab Pro is a high-end post-processing glitch suite for realistic broadcast and digital-TV signal failures, including block artifacts, line tears, RGB split, VHS noise and codec/data breakup.

QUICK START (AFTER IMPORT)
You can enable the effects in two ways:

1. AUTOMATIC SETUP

* Menu: Tools → GlitchLabProSetupTool → Add
* The tool adds the required Render Features to your active URP Renderer asset.

2. MANUAL SETUP

* Open your active URP Renderer asset:
  Project Settings → Graphics → Scriptable Render Pipeline Settings → open your URP Renderer (e.g., ForwardRenderer).
* In the Renderer asset, click “Add Render Feature”.
* Add the Glitch Lab Pro Render Features you plan to use.
* On your Camera, enable “Post Processing”.
* Create a Global Volume and add Glitch Lab Pro effects.

3. Glitch Lab Pro UI Example Scene

In this example scene you will see how to use Glitch Lab Pro for UI elements or any other separate layers.
How it works:

1. Make sure that you added "Glitch Lab Pro Custom Texture" Render feature to your Renderer.
2. Create Canvas with any UI elements on layer UI.
3. Create Render Texture and switch Anti-aliasing to "2 Samples".
4. Create separate camera to render image to Render Texture(Insert previously created Render Texture to "output texture" field.
5. Change background type to "Color".(Solid Black)
6. Change Volume Mask to any layer(for example UI)
7. Create post process volume to apply effects to UI elements.(Change Volume layer to needed(for example UI))
8. Create main post process volume.
9. In Main Volume click "Add Override" Glitch Lab Pro/Glitch Lab Pro CustomTexture.
10. Insert previously created Render Texture in "Texture" field.
11. If you want to chage render texture resolution to your current resolution automatically, Click "Add Component/ Render Texture Auto Resolution". Insert previously created Render Texture in "Texture" field.
12. All done! Now you can see your UI elements with effects rendered separately from other elements in scene.

NOTICE: Look at Glitch Lab Pro render features order in your Renderer. If you want separate layer to be rendered on top of other layers, make sure that "Custom Texture" Render feature is in the bottom of the list.

PROJECT STRUCTURE
Required folders (do not remove):

* Resources — shaders used by effects.
* Scripts — render features, passes, volumes, editor tooling.

Optional (safe to delete if not needed in your project):

* ExampleScene — remove after testing.

Note: removing Resources or Scripts will break the asset.


UNINSTALL
Before deleting files, run:
Tools → GlitchLabProSetupTool → Remove
This cleans up added Render Features and references.

SUPPORT
Email: seedodevelopment@gmail.com

Please include:

* Unity version
* Target platform (Windows, Android, iOS, etc.)
* A short video or screenshot, if possible

Bug reports are welcome. Clear, minimal reproduction steps from a new scene result in the fastest fix.