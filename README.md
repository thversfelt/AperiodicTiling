# Aperiodic Tiling

## Table of contents

1. [Introduction](#1-introduction)
2. [Features](#2-features)
3. [Screenshots](#3-screenshots)
4. [Usage](#4-usage)
5. [Support](#5-support)

## 1. Introduction

A common technique for texturing large planes is to periodically tile a single texture, which often results in noticeable and distracting periodic patterns. The Aperiodic Tiling plugin contains a Unity surface shader that removes these noticeable tiling patterns by implementing a state-of-art stochastic tiling system as introduced in the paper by [Cohen et al](http://www.cs.jhu.edu/~misha/Fall19/Readings/Cohen03.pdf).

## 2. Features

- Pattern generator to generate random tilings
- Normal mapping support
- Adjustable Level-of-Detail (LOD)
- Demo scene
- Three example tilesets (basic, flowers and rocks)
- Easy to use user-interface
- Clean, well-documented code

## 3. Screenshots

The following screenshots are taken within the included demo scene and demonstrate the removal of the noticable tiling pattern.

![Basic](https://user-images.githubusercontent.com/40113382/90249906-90ae4c00-de3b-11ea-9fca-06894c1cb7e5.jpg)

![Flowers](https://user-images.githubusercontent.com/40113382/90249916-9310a600-de3b-11ea-9d8a-e6c2077cca38.jpg)

![Rocks](https://user-images.githubusercontent.com/40113382/90332145-d2b1cc00-dfba-11ea-8ba6-215a3e70f47c.jpg)

## 4. Usage

The following [tutorial video](https://youtu.be/zU1IxlMyJv8) shows how to setup the plugin and use Rob Burke's [Wang tiler](https://robburke.net/mle/wang/) to create a new custom tileset from a single source image.

## 5. Support

If you encounter any bugs, please create an issue with a description and the steps to reproduce the bug on the [issues board](https://github.com/thversfelt/AperiodicTiling/issues). If you need additional support, send me an [e-mail](mailto:thversfelt@hotmail.com).
