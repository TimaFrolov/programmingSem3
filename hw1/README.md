# ДЗ 1
Запуск:
- Само приложение `dotnet run --project MatrixMultiplication`
- Бенчмарк `dotnet run --project Tester --configuration Release`
Результаты запуска бенчмарка на моём ноутбуке (Lenovo Yoga 7i, Intel Core i5-1135G7, linux fedora, Balanced power mode, dotnet version 7.0.110):

|                    Method | size |                 Mean |                Error |               StdDev |               Median |
|---------------------------|------|---------------------:|---------------------:|---------------------:|---------------------:|
|                  Multiply |    1 |             38.80 ns |             0.296 ns |             0.277 ns |             38.86 ns |
| MultiplyWithTransposition |    1 |             75.83 ns |             0.579 ns |             0.542 ns |             75.93 ns |
|       MultiplyWithThreads |    1 |         75,898.19 ns |           585.684 ns |           519.193 ns |         75,855.94 ns |
|                  Multiply |    2 |             55.41 ns |             0.352 ns |             0.294 ns |             55.40 ns |
| MultiplyWithTransposition |    2 |             98.95 ns |             0.529 ns |             0.495 ns |             98.90 ns |
|       MultiplyWithThreads |    2 |        145,564.57 ns |           835.911 ns |           781.911 ns |        145,578.54 ns |
|                  Multiply |    4 |            178.18 ns |             0.900 ns |             0.842 ns |            178.20 ns |
| MultiplyWithTransposition |    4 |            243.35 ns |             1.802 ns |             1.597 ns |            243.34 ns |
|       MultiplyWithThreads |    4 |        278,128.84 ns |         2,565.710 ns |         2,399.966 ns |        278,168.63 ns |
|                  Multiply |    8 |          1,178.56 ns |            12.054 ns |            11.276 ns |          1,175.05 ns |
| MultiplyWithTransposition |    8 |          1,306.45 ns |            17.442 ns |            15.462 ns |          1,305.31 ns |
|       MultiplyWithThreads |    8 |        539,142.91 ns |         3,767.445 ns |         3,524.071 ns |        539,052.46 ns |
|                  Multiply |   16 |          8,742.76 ns |            23.672 ns |            20.985 ns |          8,742.41 ns |
| MultiplyWithTransposition |   16 |         10,510.74 ns |            39.478 ns |            34.996 ns |         10,520.44 ns |
|       MultiplyWithThreads |   16 |      1,065,622.75 ns |         7,148.996 ns |         6,687.175 ns |      1,064,441.58 ns |
|                  Multiply |   32 |         71,305.05 ns |           198.100 ns |           175.611 ns |         71,333.56 ns |
| MultiplyWithTransposition |   32 |         74,125.80 ns |           345.747 ns |           288.714 ns |         74,147.84 ns |
|       MultiplyWithThreads |   32 |      2,119,997.34 ns |        19,332.300 ns |        17,137.580 ns |      2,118,472.05 ns |
|                  Multiply |   64 |        548,599.38 ns |         3,994.901 ns |         3,736.833 ns |        548,745.27 ns |
| MultiplyWithTransposition |   64 |        556,767.70 ns |         4,103.621 ns |         3,838.529 ns |        557,649.27 ns |
|       MultiplyWithThreads |   64 |      4,306,685.08 ns |        48,659.768 ns |        45,516.377 ns |      4,299,881.48 ns |
|                  Multiply |  128 |      4,414,005.42 ns |        20,983.080 ns |        18,600.954 ns |      4,407,593.75 ns |
| MultiplyWithTransposition |  128 |      4,323,020.93 ns |        25,829.625 ns |        22,897.290 ns |      4,332,386.57 ns |
|       MultiplyWithThreads |  128 |      9,096,498.82 ns |       180,536.486 ns |       185,397.679 ns |      9,037,738.55 ns |
|                  Multiply |  256 |     36,208,403.85 ns |       444,385.765 ns |       415,678.721 ns |     36,230,345.57 ns |
| MultiplyWithTransposition |  256 |     34,542,157.91 ns |       415,038.695 ns |       346,576.108 ns |     34,564,375.13 ns |
|       MultiplyWithThreads |  256 |     33,811,473.69 ns |       336,235.256 ns |       298,063.796 ns |     33,893,849.47 ns |
|                  Multiply |  512 |    347,068,909.53 ns |     1,731,876.808 ns |     1,619,998.641 ns |    346,511,296.00 ns |
| MultiplyWithTransposition |  512 |    268,837,732.09 ns |     1,280,550.475 ns |     1,257,671.791 ns |    269,106,471.50 ns |
|       MultiplyWithThreads |  512 |    246,816,863.41 ns |     9,733,119.891 ns |    28,698,323.599 ns |    237,905,873.17 ns |
|                  Multiply | 1024 |  3,779,655,735.08 ns |   125,794,841.959 ns |   364,953,646.177 ns |  3,573,512,544.00 ns |
| MultiplyWithTransposition | 1024 |  2,340,578,059.03 ns |    65,968,403.844 ns |   193,473,908.912 ns |  2,251,330,384.00 ns |
|       MultiplyWithThreads | 1024 |  2,130,587,135.11 ns |   108,538,536.174 ns |   320,028,322.791 ns |  2,018,189,797.50 ns |
|                  Multiply | 2048 | 88,545,278,505.33 ns | 1,754,500,458.749 ns | 1,641,160,817.684 ns | 88,099,554,029.00 ns |
| MultiplyWithTransposition | 2048 | 19,642,799,323.58 ns |   481,852,043.343 ns | 1,420,751,621.495 ns | 19,712,727,729.50 ns |
|       MultiplyWithThreads | 2048 | 19,554,523,615.69 ns |    40,728,629.092 ns |    34,010,249.900 ns | 19,556,726,726.00 ns |
