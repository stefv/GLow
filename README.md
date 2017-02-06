# GLow
GLow is an open source screensaver for Windows using shaders. The shaders are small programs for the graphics card of your computer. Very good quality shaders can be downloaded automatically from the website ShaderToy to offer you beautiful animations (2D and 3D) on the screen when you are not using the computer.

# Contribute
To contribute to this project, you need to use Microsoft Visual Studio 2015 (Community Edition is enough). You can clone this project to your IDE. When the project will be cloned, an error will be rised from the IDE about a resource file: _PrivateData.resx_. This file contains the key to access to the database of ShaderToy. You need to create a key on this web site: https://www.shadertoy.com/api

When you have your key, just create a resource file in the IDE with the name _PrivateData.resx_. This file must never be pushed to the GIT repository.

Here the steps to create the resource file in Visual Studio 2015 (french version).

## Step 1 : create the resource file
![Process to create the resource file](https://raw.githubusercontent.com/stefv/GLow/master/Site/Images/create-the-file.png)

## Step 2 : create the string resource
![Process to create the string resource for the key](https://raw.githubusercontent.com/stefv/GLow/master/Site/Images/create-the-key.png)

After this second step, you can compile the project.
