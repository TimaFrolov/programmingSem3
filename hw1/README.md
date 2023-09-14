# ДЗ 1
Запуск:
- Само приложение `dotnet run --project MatrixMultiplication`
- Бенчмарк `dotnet run --project Tester --configuration Release`
Результаты запуска бенчмарка на моём ноутбуке (Lenovo Yoga 7i, Intel Core i5-1135G7, linux fedora, Balanced power mode, dotnet version 7.0.110):

|                    Method | size |                 Mean |              Error |               StdDev |               Median |
|-------------------------- |----- |---------------------:|-------------------:|---------------------:|---------------------:|
|                  Multiply |    1 |             51.65 ns |           0.541 ns |             0.506 ns |             51.68 ns |
| MultiplyWithTransposition |    1 |             97.40 ns |           1.745 ns |             1.633 ns |             97.38 ns |
|       MultiplyWithThreads |    1 |        112,355.22 ns |       2,199.356 ns |         3,793.781 ns |        111,882.22 ns |
|                  Multiply |    2 |             73.62 ns |           1.518 ns |             3.067 ns |             73.39 ns |
| MultiplyWithTransposition |    2 |            129.41 ns |           2.646 ns |             5.581 ns |            127.33 ns |
|       MultiplyWithThreads |    2 |        213,880.77 ns |       2,425.188 ns |         2,268.522 ns |        213,821.02 ns |
|                  Multiply |    4 |            187.38 ns |           3.031 ns |             3.941 ns |            188.15 ns |
| MultiplyWithTransposition |    4 |            267.45 ns |           4.426 ns |             4.140 ns |            266.27 ns |
|       MultiplyWithThreads |    4 |        398,710.64 ns |       7,933.852 ns |        18,701.022 ns |        406,104.77 ns |
|                  Multiply |    8 |          1,183.20 ns |          18.319 ns |            21.807 ns |          1,176.66 ns |
| MultiplyWithTransposition |    8 |          1,343.95 ns |           9.471 ns |             7.908 ns |          1,341.15 ns |
|       MultiplyWithThreads |    8 |        712,747.89 ns |      13,407.258 ns |        16,465.317 ns |        718,197.25 ns |
|                  Multiply |   16 |         10,495.06 ns |         100.544 ns |            89.130 ns |         10,503.48 ns |
| MultiplyWithTransposition |   16 |         11,429.91 ns |         190.355 ns |           178.058 ns |         11,394.59 ns |
|       MultiplyWithThreads |   16 |      1,106,616.70 ns |       8,819.490 ns |         7,818.248 ns |      1,109,222.14 ns |
|                  Multiply |   32 |         73,350.19 ns |         448.584 ns |           397.658 ns |         73,411.89 ns |
| MultiplyWithTransposition |   32 |         76,049.71 ns |         667.162 ns |           624.064 ns |         76,375.66 ns |
|       MultiplyWithThreads |   32 |      3,057,934.01 ns |      22,688.648 ns |        18,946.049 ns |      3,054,700.95 ns |
|                  Multiply |   64 |        656,535.98 ns |      11,264.545 ns |        10,536.863 ns |        653,887.08 ns |
| MultiplyWithTransposition |   64 |        565,399.46 ns |       4,147.210 ns |         3,237.868 ns |        564,493.79 ns |
|       MultiplyWithThreads |   64 |      4,413,571.96 ns |      24,437.349 ns |        22,858.712 ns |      4,420,332.52 ns |
|                  Multiply |  128 |      4,489,102.87 ns |      26,453.960 ns |        22,090.255 ns |      4,489,450.15 ns |
| MultiplyWithTransposition |  128 |      4,397,511.51 ns |      30,765.673 ns |        25,690.730 ns |      4,398,671.86 ns |
|       MultiplyWithThreads |  128 |     13,768,795.98 ns |     268,175.453 ns |       250,851.485 ns |     13,638,801.16 ns |
|                  Multiply |  256 |     43,438,565.24 ns |     825,341.821 ns |       810,596.026 ns |     43,208,517.25 ns |
| MultiplyWithTransposition |  256 |     34,795,194.67 ns |     197,182.844 ns |       184,444.955 ns |     34,808,392.46 ns |
|       MultiplyWithThreads |  256 |     42,541,415.18 ns |   1,671,058.728 ns |     4,927,154.363 ns |     41,822,308.87 ns |
|                  Multiply |  512 |    315,520,256.80 ns |   4,909,347.272 ns |    11,378,158.823 ns |    311,317,512.00 ns |
| MultiplyWithTransposition |  512 |    275,178,940.58 ns |   2,086,409.959 ns |     2,712,922.284 ns |    275,336,514.00 ns |
|       MultiplyWithThreads |  512 |    193,380,724.61 ns |   3,839,750.279 ns |     4,715,558.174 ns |    195,895,608.00 ns |
|                  Multiply | 1024 |  3,825,902,997.71 ns |  20,969,144.786 ns |    18,588,600.670 ns |  3,824,406,092.00 ns |
| MultiplyWithTransposition | 1024 |  2,448,692,469.93 ns |  21,705,234.641 ns |    20,303,089.950 ns |  2,448,005,093.00 ns |
|       MultiplyWithThreads | 1024 |  1,729,404,227.92 ns |  40,362,379.062 ns |   111,169,713.853 ns |  1,768,764,004.50 ns |
|                  Multiply | 2048 | 78,958,627,367.27 ns | 782,929,152.883 ns |   732,352,415.370 ns | 79,031,074,287.00 ns |
| MultiplyWithTransposition | 2048 | 19,486,107,812.69 ns |  22,555,464.408 ns |    18,834,834.323 ns | 19,489,894,671.00 ns |
|       MultiplyWithThreads | 2048 | 16,950,517,995.79 ns | 769,854,390.625 ns | 2,269,933,040.458 ns | 16,928,307,917.00 ns |
