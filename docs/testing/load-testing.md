# Результаты нагрузочного тестирования

Приведем результаты нагрузочного тестирования для нескольких конечных точек соединения. Замеры выполнядись с помощью утилиты Apache Benchmark. Для тестирования были выбраны параметры n = 1000 и c = 100 (выполнение 1000 запросов при числе одновременных 100).

## /api/v1/users/me

### Без балансировки

RPS: 569.52

p50 = 139

p95 = 211

p99 = 246

Время подключения в мс:

|             | mean | [+/-sd] | Выход | median | max |
|-------------|------|---------|-------|--------|-----|
| Connect:    | 0    | 1       | 1.9   | 1      | 18  |
| Processing: | 49   | 142     | 38.4  | 138    | 306 |
| Waiting:    | 49   | 139     | 38.3  | 136    | 306 |
| Total:      | 50   | 143     | 38.4  | 139    | 309 |

### С балансировкой

RPS: 933.55

p50 = 105

p95 = 115

p99 = 117

Время подключения в мс:

|             | mean | [+/-sd] | Выход | median | max |
|-------------|------|---------|-------|--------|-----|
| Connect:    | 0    | 1       | 1.9   | 0      | 8   |
| Processing: | 9    | 101     | 17.9  | 105    | 120 |
| Waiting:    | 7    | 99      | 17.8  | 103    | 118 |
| Total:      | 15   | 102     | 16.3  | 105    | 125 |