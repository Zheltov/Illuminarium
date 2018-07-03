# Illuminarium

C# implementation of the classic and view independent algorithm for visualization of 3D scenes by local estimations of the Monte Carlo method. 

Реализация двойной локальной оценки метода Монте-Карло для визуализации трехмерных сцен на C# и реализация видо-незвисимой локальной оценки - аналога radiosity но не для уравнения излучательности, а для уравнения глобального освещения. Локальные оценки также известны как instant radiosity.

*Проект создан не как конечный продукт, а только тот минимум, который был неодходим в рамках работы над диссертацией*

Головной проект: Illuminarium

## Запуск
### Render Double Local Estimation
Почти классический рендер локальными оценками, но расчет ведется не в точках проекциях пиксельной матрицы, а также как и в radiosity в узлах сетки. После чего происходит усреднение по пакетам и пересчет для каждой точки пиксельной матрицы. По сути есть ответвление от View Independent Double Local Estimation, но безперспективное
Настройки:
```
Scene = new TestScenes.SceneCreatorCornellBox().CreateScene();   // Сцена
int nPackets = 3;       // Число пакетов
int nRays = 100;        // Число лучей внутри пакета для двойной локальной
float wMin = 0.1f;      // Минимульный вес луча марковской цепи для локальной
```

### View Independent Double Local Estimation
Видонезависимая локальная оценка. После расчета можно крутить камеру и яркость будет пересчитываться в реальном времени для различного положения камеры. Аппроксимация в частности реализована на сферических гармониках. Так как алгоритм мягко говоря не быстрый, то для примера расчитывается простейшая сцена задачи Соболева (параллельные плоскости и изотропный источник между ними).
Настройки:
```
Scene = new TestScenes.SceneCreatorSobolevEx().CreateScene();   // Сцена
int maxDivideDeep = 10; // Максимальная глубина разбиения грани, больше 14 лучше не задавать даже для Соболева
int nTheta = 8;         // Число углов разбиения для расчета яркости по зенитному углу
int nPhi = 16;          // Число углов разбиения для расчета яркости по азимутальному углу
int nSH = 8;            // Число гармоник при аппроксимации через СГ (MVertexIlluminanceApproximationMode.SphericalHarmonics)
int nRays = 100;        // Число лучей для двойной локальной
float wMin = 0.1f;      // Минимульный вес луча марковской цепи для локальной
```

## Зависимости:
Все включено в проект, но используются:
- Embree
- https://github.com/TomCrypto/Embree.NET
- http://www.alglib.net/
- opengl


## Подробнее локальных оценках

- Budak V.P., Zheltov V.S., Lubenchenko A.V., Freidlin K.S., Shagalov O.V. A FAST AND ACCURATE SYNTHETIC ITERATION-BASED ALGORITHM FOR NUMERICAL SIMULATION OF RADIATIVE TRANSFER IN A TURBID MEDIUM // Atmospheric and Oceanic Optics. 2017. Т. 30. № 1. С. 70-78. 
- Budak V., Zheltov V., Notfulin R., Chembaev V. RELATION OF INSTANT RADIOSITY METHOD WITH LOCAL ESTIMATIONS OF MONTE CARLO METHOD // Journal of WSCG. 2016. 
- Zheltov V.S., Budak V.P. LOCAL MONTE CARLO ESTIMATION METHODS IN THE SOLUTION OF GLOBAL ILLUMINATION EQUATION // В сборнике: 22nd International Conference in Central Europe on Computer Graphics, Visualization and Computer Vision, WSCG 2014, Communication Papers Proceedings - in co-operation with EUROGRAPHICS Association 22. 2015. С. 25-30. 
- Чембаев В.Д., Будак В.П., Желтов В.С., Нотфулин Р.С., Селиванов В.А. USAGE OF LOCAL ESTIMATIONS AT THE SOLUTION OF GLOBAL ILLUMINATION EQUATION // В сборнике: ГРАФИКОН'2015 Труды Юбилейной 25-й Международной научной конференции. 2015. С. 7-11. 
- Budak V.P., Zheltov V.S., Lubenchenko A.V., Shagalov O.V. ON THE EFFICIENCY OF ALGORITHMS OF MONTE CARLO METHODS // В сборнике: Proceedings of SPIE - The International Society for Optical Engineering 21, Atmospheric Physics. Сер. "21st International Symposium on Atmospheric and Ocean Optics: Atmospheric Physics" 2015. С. 96801P. 
- Будак В.П., Желтов В.С. ЛОКАЛЬНЫЕ ОЦЕНКИ МЕТОДА МОНТЕ-КАРЛО В МОДЕЛИРОВАНИИ УРАВНЕНИЯ ГЛОБАЛЬНОГО ОСВЕЩЕНИЯ // В сборнике: Проблемы и перспективы развития отечественной светотехники, электротехники и энергетики материалы XII Всероссийской научно-технической конференции с международным участием в рамках III Всероссийского светотехнического форума с международным участием. Ответственный редактор О. Е. Железникова; Мордовский государственный университет имени Н. П. Огарёва. 2015. С. 92-96. 

## Примеры визуализаций:
![][img01]
![][img02]
![][img03]

[img01]: https://github.com/Zheltov/Illuminarium/blob/master/Images/01.png
[img02]: https://github.com/Zheltov/Illuminarium/blob/master/Images/vi_01.png
[img03]: https://github.com/Zheltov/Illuminarium/blob/master/Images/vi_02.png