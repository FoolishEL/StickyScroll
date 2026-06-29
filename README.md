<div align="center">
  <h1>StickyScroll</h1>
  
  <p>
    <strong>Infinite scroll с рециклингом элементов для Unity</strong>
  </p>

  <p>
    <img src="https://img.shields.io/badge/Unity-2022.3+-000000?logo=unity&logoColor=white" alt="Unity" />
    <img src="https://img.shields.io/badge/URP-Ready-4CAF50" alt="URP" />
    <img src="https://img.shields.io/badge/VContainer-Supported-9C27B0" alt="VContainer" />
    <img src="https://img.shields.io/github/license/FoolishEL/StickyScroll" alt="License" />
  </p>
  <img width="640" height="480" alt="SampleGif" src="https://github.com/user-attachments/assets/f1515e7c-b6d3-4cae-9f64-d89457675eed" />



</div>

## О проекте

**StickyScroll** — это лёгкий и производительный компонент для создания бесконечных прокручиваемых списков в Unity. Использует технику **object recycling** (всего 3 активных контейнера).

### Основные возможности

- Пул из 3 элементов (минимальный GC и отличная производительность)
- Плавный drag + инерция
- Поддержка верхнего и нижнего лимита данных
- Простое расширение через наследование `AbstractContentContainer`
- Интеграция с DI (VContainer)
- Работает поверх RectTransform (совместим с Canvas)
