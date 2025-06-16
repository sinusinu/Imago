# Imago

A personal CLI tool/framework for batch manipulating image files.

The idea is that you define a pipeline of image-manipulating actions, and image files are run through it.

# Disclaimer

This tool is:

- made without a piece of quality in mind
- intended for personal use only

so:

- there won't be any support.
- I wouldn't use it if I were you.

# Usage

```sh
imago [filename|foreach filter|foreach directory filter] run [actions...]
```

# Actions

Basic syntax of action goes like this:

```
identifier?option1=value?option2=value...
```

Currently following actions are available:

- Load
  - Identifier: `load`
  - Must be called first
  - No options

- Resize
  - Identifier: `resize`
  - Options:
    - `w`: Width, positive integer, optional
    - `h`: Height, positive integer, optional
    - `filter`: Use bilinear filter, true or false, optional
  - If both `w` and `h` are given, aspect ratio will be ignored
  - If only one of `w` or `h` is given, aspect ratio will be kept and missing axis will be calculated
  - Either one of `w` or `h` must be given
  - `filter=true` (default) will use bilinear filtering, `filter=false` will use nn filtering

- Save
  - Identifier: `save`
  - Options:
    - `format`: One of `bmp`, `jpg`, `gif`, `png`, `webp`
    - `quality`: Used if set format uses quality option (is lossy format), integer, optional (default: 100)

# Example usage

```sh
# Single file, convert jpeg to loseless webp
imago image.jpg run load save?format=webp

# Batch (on current dir), convert jpeg to loseless webp
imago foreach *.jpg run load save?format=webp

# Batch (on other dir), resize by width of 320, convert jpeg to lossy webp with quality of 80
imago foreach "C:\some\path" *.jpg run load resize?w=320 save?format=webp?quality=80
```

# License

Imago is distributed under the GNU GPL v3 License.