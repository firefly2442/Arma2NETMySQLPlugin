#!/bin/bash

#This system uses Sphinx to generate HTML documentation
#http://sphinx-doc.org

#General setup:
#sudo pip install sphinx
#sudo pip install sphinx-intl

#This sets up the appropriate folders and config file
#sphinx-quickstart

#Edit conf.py and add the following entries:
#locale_dirs = ['locale/']   # path is example but recommended.
#gettext_compact = False     # optional.

#Create or update .pot file (found in build directory)
make gettext

#Change directory to source
cd source

#Create .po files for each language we want
#A list of the supported languages for Sphinx can be found here:
#http://sphinx-doc.org/config.html#confval-language
#sphinx-intl update -p ../build/locale -l de -l ja -l cs -l da -l fr -l es -l it -l ru -l sv -l nb_NO -l uk_UA -l pt_BR -l zh_CN -l zh_TW

#Update .po files from our new .pot file
sphinx-intl update -p ../build/locale

#Generate .mo files (these are binary versions of .po files)
sphinx-intl build

#Go back to main directory
cd ../

#Generate documentation for each language
make -e BUILDDIR="docs/en" singlehtml #default English
make -e SPHINXOPTS=" -D language='de'" BUILDDIR="docs/de" singlehtml #German
make -e SPHINXOPTS=" -D language='ja'" BUILDDIR="docs/ja" singlehtml #Japanese
make -e SPHINXOPTS=" -D language='cs'" BUILDDIR="docs/cs" singlehtml #Czech
make -e SPHINXOPTS=" -D language='da'" BUILDDIR="docs/da" singlehtml #Danish
make -e SPHINXOPTS=" -D language='fr'" BUILDDIR="docs/fr" singlehtml #French
make -e SPHINXOPTS=" -D language='es'" BUILDDIR="docs/es" singlehtml #Spanish
make -e SPHINXOPTS=" -D language='it'" BUILDDIR="docs/it" singlehtml #Italian
make -e SPHINXOPTS=" -D language='ru'" BUILDDIR="docs/ru" singlehtml #Russian
make -e SPHINXOPTS=" -D language='sv'" BUILDDIR="docs/sv" singlehtml #Swedish
make -e SPHINXOPTS=" -D language='nb_NO'" BUILDDIR="docs/nb_NO" singlehtml #Norwegian Bokmal
make -e SPHINXOPTS=" -D language='uk_UA'" BUILDDIR="docs/uk_UA" singlehtml #Ukrainian
make -e SPHINXOPTS=" -D language='pt_BR'" BUILDDIR="docs/pt_BR" singlehtml #Brazilian Portuguese
make -e SPHINXOPTS=" -D language='zh_CN'" BUILDDIR="docs/zh_CN" singlehtml #Simplified Chinese
make -e SPHINXOPTS=" -D language='zh_TW'" BUILDDIR="docs/zh_TW" singlehtml #Traditional Chinese
