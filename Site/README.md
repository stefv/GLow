# Technical documentation
## Architecture
GLow is composed of 3 different components. This architecture grant to the users to share the common data avoiding to download many times (per user). To do this task, GLow is using a Windows service with a local database. This service will download the shaders from [ShaderToys](https://www.shadertoy.com/) and share them to the screensaver used by the different users of the PC.

![The components](https://github.com/stefv/GLow/raw/Windows10_UI/Site/Images/architecture.png)

## GLow Windows service
This Windows service is installed during the setup of GLow.
