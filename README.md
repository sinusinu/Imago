# Imago

A personal tool/framework for batch manipulating image files.

Currently only format conversion can be done; might add more things later (like resize or sth)

# Disclaimer

This tool is:

- made very hastily
- intended for personal use only (for now)

so:

- there won't be any support.
- I wouldn't use it rn if I were you.

# Example usage

```sh
# Single file format conversion
imago image.jpg run load save?format=webp

# Batch file format conversion (on current dir)
imago foreach *.jpg run load save?format=webp

# Batch file format conversion (on other dir)
imago foreach "C:\some\path" *.jpg run load save?format=webp?quality=80
```

# License

Imago is distributed under the GNU GPL v3 License.