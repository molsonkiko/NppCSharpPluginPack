: Requires ImageMagick (https://imagemagick.org/index.php) to be installed.
: add toolbar bitmaps
magick "selection remembering form icon.png" -resize 20x20 "selection remembering form toolbar bmp.bmp"
magick "about form icon.png" -resize 20x20 "about form toolbar bmp.bmp"
magick "close html tag icon.png" -resize 20x20 "close html tag toolbar bmp.bmp"

: add toolbar icons
magick "selection remembering form icon.png" -resize 20x20 "selection remembering form toolbar.ico"
magick "about form icon.png" -resize 20x20 "about form toolbar.ico"
magick "close html tag icon.png" -resize 20x20 "close html tag toolbar.ico"

: add darkmode icons
magick "selection remembering form icon darkmode.png" -resize 20x20 "selection remembering form toolbar darkmode.ico"
magick "about form toolbar darkmode.png" -resize 20x20 "about form toolbar darkmode.ico"
magick "close html tag icon darkmode.png" -resize 20x20 "close html tag toolbar darkmode.ico"