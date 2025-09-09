# 📚 ToDo API - Pagination, Sorting & Filtering

## 🚀 **Нові можливості API**

Ваш ToDo API тепер підтримує професійні функції:
- **📄 Pagination** - розбиття на сторінки
- **🔄 Sorting** - сортування за різними полями  
- **🔍 Filtering** - фільтрація за критеріями
- **🔎 Search** - пошук по тексту

## 📖 **Документація endpoints**

### **🔹 GET /todoitems - Головний endpoint з усіма функціями**

#### **Параметри запиту:**

| Параметр | Тип | За замовчуванням | Опис |
|----------|-----|------------------|------|
| `pageNumber` | int | 1 | Номер сторінки |
| `pageSize` | int | 10 | Кількість записів на сторінці |
| `sortBy` | string | "createddate" | Поле для сортування |
| `sortDirection` | string | "desc" | Напрямок сортування (asc/desc) |
| `priority` | Priority? | null | Фільтр за пріоритетом (1=Low, 2=Medium, 3=High) |
| `isComplete` | bool? | null | Фільтр за статусом (true/false) |
| `searchQuery` | string? | null | Пошук в назві та описі |

#### **Поля для сортування:**
- `name` - за назвою
- `priority` - за пріоритетом
- `duedate` - за датою дедлайну
- `iscomplete` - за статусом завершення  
- `createddate` - за датою створення (default)

## 🌐 **Приклади використання**

### **📄 Базова пагінація:**
```http
GET /todoitems?pageNumber=1&pageSize=5
```

### **🔄 Сортування:**
```http
# За пріоритетом (від високого до низького)
GET /todoitems?sortBy=priority&sortDirection=desc

# За назвою (алфавітний порядок)
GET /todoitems?sortBy=name&sortDirection=asc

# За датою створення (нові спочатку)
GET /todoitems?sortBy=createddate&sortDirection=desc
```

### **🔍 Фільтрація:**
```http
# Тільки високий пріоритет
GET /todoitems?priority=3

# Тільки завершені задачі
GET /todoitems?isComplete=true

# Незавершені задачі
GET /todoitems?isComplete=false
```

### **🔎 Пошук:**
```http
# Пошук по тексту
GET /todoitems?searchQuery=робота

# Пошук по частині слова
GET /todoitems?searchQuery=про
```

### **🎯 Комбінації:**
```http
# Складний запит: незавершені задачі високого пріоритету з пошуком "проект", 
# відсортовані за дедлайном, друга сторінка по 5 записів
GET /todoitems?pageNumber=2&pageSize=5&sortBy=duedate&sortDirection=asc&priority=3&isComplete=false&searchQuery=проект
```

## 📊 **Формат відповіді**

### **Нова структура відповіді (з пагінацією):**
```json
{
  "data": [
    {
      "id": 1,
      "name": "Важлива задача",
      "description": "Детальний опис",
      "isComplete": false,
      "createdDate": "2024-01-15T14:30:00Z",
      "dueDate": "2024-02-01",
      "priority": "High",
      "tags": ["робота", "терміново"]
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### **Метадані пагінації:**
- `pageNumber` - поточна сторінка
- `pageSize` - розмір сторінки  
- `totalCount` - загальна кількість записів
- `totalPages` - загальна кількість сторінок
- `hasNextPage` - чи є наступна сторінка
- `hasPreviousPage` - чи є попередня сторінка

## 🔄 **Backward Compatibility**

### **🔹 GET /todoitems/all - Всі записи без пагінації**
```http
GET /todoitems/all
```
Повертає старий формат - масив всіх записів для сумісності зі старим кодом.

### **🔹 GET /todoitems/complete - Завершені задачі**  
```http
GET /todoitems/complete
```
Як і раніше повертає тільки завершені задачі.

## 🎨 **Приклади для Frontend**

### **JavaScript fetch приклади:**

```javascript
// Базове завантаження з пагінацією
async function loadTodos(page = 1) {
    const response = await fetch(`/todoitems?pageNumber=${page}&pageSize=10`);
    const result = await response.json();
    
    console.log('Задачі:', result.data);
    console.log('Сторінка:', result.pageNumber, 'з', result.totalPages);
    console.log('Всього:', result.totalCount);
}

// Пошук з автокомплітом
async function searchTodos(query) {
    const response = await fetch(`/todoitems?searchQuery=${encodeURIComponent(query)}&pageSize=5`);
    const result = await response.json();
    return result.data;
}

// Фільтрація за пріоритетом
async function loadHighPriorityTodos() {
    const response = await fetch('/todoitems?priority=3&sortBy=duedate&sortDirection=asc');
    const result = await response.json();
    return result.data;
}

// Повна функціональність
async function loadTodosAdvanced(filters) {
    const params = new URLSearchParams();
    
    if (filters.page) params.append('pageNumber', filters.page);
    if (filters.size) params.append('pageSize', filters.size);
    if (filters.sort) params.append('sortBy', filters.sort);
    if (filters.direction) params.append('sortDirection', filters.direction);
    if (filters.priority) params.append('priority', filters.priority);
    if (filters.completed !== undefined) params.append('isComplete', filters.completed);
    if (filters.search) params.append('searchQuery', filters.search);
    
    const response = await fetch(`/todoitems?${params}`);
    return await response.json();
}
```

## 🚀 **Переваги нових функцій**

### **📄 Pagination:**
- ✅ Швидше завантаження сторінок
- ✅ Менше навантаження на сервер
- ✅ Краща UX для великих списків

### **🔄 Sorting:**
- ✅ Гнучке сортування за будь-яким полем
- ✅ Висхідний та спадний порядок
- ✅ Інтуїтивна логіка для користувачів

### **🔍 Filtering:**
- ✅ Швидкий пошук потрібних задач
- ✅ Комбінування кількох фільтрів
- ✅ Точна фільтрація за статусом та пріоритетом

### **🔎 Search:**
- ✅ Пошук по назві та опису
- ✅ Часткове співпадіння
- ✅ Швидкий доступ до потрібної інформації

---

**🎯 Ваш ToDo API тепер має професійний рівень функціональності!** 🌟
