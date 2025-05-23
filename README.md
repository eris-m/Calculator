# Calculator.

A simple calculator app, made with avalonia!

## Features.

- 4 function arithmetic, with proper order of operations,
- parenthesis,
- some functions,
- and, some constants. 

## Supported functions.

Currently, the following functions are supported:

| name         | description                                                     |
|--------------|-----------------------------------------------------------------|
| `pow(x, n)`  | Raises `x` to the power of `n`.                                 |
| `sqrt(x)`    | Square root of `x`. Special case of `root(x)`.                  |
| `root(x, n)` | The `n`th root of `x`. `n` is rounded to the nearest integer.   |
| `sin(x)`     | Sine of `x`.                                                    |
| `cos(x)`     | Cosine of `x`.                                                  |
| `tan(x)`     | Tangent of `x`.                                                 |
| `asin(x)`    | Inverse sine of `x`.                                            |
| `acos(x)`    | Inverse cosine of `x`.                                          |
| `atan(x)`    | Inverse tangent of `x`.                                         |
| `min(x, y)`  | Minimum of `x` and `y`.                                         |
| `max(x, y)`  | Maximum of `x` and `y`.                                         |
| `floor(x)`   | Rounds `x` down to the nearest integer.                         |
| `ceil(x)`    | Rounds `x` up to the nearest integer.                           |

## Constants

There are 4 constants defined by default:

| name   | approx value |                          description                           |
| ------ | ------------ | -------------------------------------------------------------  |
| `pi`   |   3.14159    | π, the ratio between a circle's circumference to its diameter. |
| `tau`  |   6.28318    |   τ, the ratio between a circle's circumference and radius.    |
| `e`    |              |         e, the natural logarithm/exponent constant.            |
| `gold` |              |                      The golden ratio.                         |