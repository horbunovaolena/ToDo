// API base URL - adjust if your API runs on a different port
const API_BASE_URL = '/todoitems';

// Global variables
let todos = [];
let currentFilter = 'all';

// DOM elements
const todoInput = document.getElementById('todoInput');
const todoList = document.getElementById('todoList');
const todoCount = document.getElementById('todoCount');
const filterButtons = document.querySelectorAll('.filter-btn');

// Initialize the app
document.addEventListener('DOMContentLoaded', function() {
    loadTodos();
    
    // Add event listener for Enter key in input
    todoInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            addTodo();
        }
    });
});

// API Functions
async function loadTodos() {
    try {
        showLoading();
        const response = await fetch(API_BASE_URL);
        if (!response.ok) {
            throw new Error('Failed to load todos');
        }
        todos = await response.json();
        renderTodos();
        updateStats();
    } catch (error) {
        showError('Failed to load todos: ' + error.message);
    }
}

async function addTodo() {
    const todoText = todoInput.value.trim();
    if (!todoText) {
        alert('Please enter a task!');
        return;
    }

    try {
        const newTodo = {
            name: todoText,
            isComplete: false
        };

        const response = await fetch(API_BASE_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(newTodo)
        });

        if (!response.ok) {
            throw new Error('Failed to add todo');
        }

        const createdTodo = await response.json();
        todos.push(createdTodo);
        todoInput.value = '';
        renderTodos();
        updateStats();
    } catch (error) {
        showError('Failed to add todo: ' + error.message);
    }
}

async function toggleTodo(id, isComplete) {
    try {
        const todo = todos.find(t => t.id === id);
        if (!todo) return;

        const updatedTodo = {
            ...todo,
            isComplete: isComplete
        };

        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(updatedTodo)
        });

        if (!response.ok) {
            throw new Error('Failed to update todo');
        }

        // Update local state
        todo.isComplete = isComplete;
        renderTodos();
        updateStats();
    } catch (error) {
        showError('Failed to update todo: ' + error.message);
    }
}

async function deleteTodo(id) {
    if (!confirm('Are you sure you want to delete this task?')) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Failed to delete todo');
        }

        // Remove from local state
        todos = todos.filter(t => t.id !== id);
        renderTodos();
        updateStats();
    } catch (error) {
        showError('Failed to delete todo: ' + error.message);
    }
}

async function editTodo(id, newName) {
    try {
        const todo = todos.find(t => t.id === id);
        if (!todo) return;

        const updatedTodo = {
            ...todo,
            name: newName
        };

        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(updatedTodo)
        });

        if (!response.ok) {
            throw new Error('Failed to update todo');
        }

        // Update local state
        todo.name = newName;
        renderTodos();
    } catch (error) {
        showError('Failed to update todo: ' + error.message);
    }
}

// UI Functions
function renderTodos() {
    const filteredTodos = getFilteredTodos();
    todoList.innerHTML = '';

    if (filteredTodos.length === 0) {
        todoList.innerHTML = '<li class="loading">No tasks found</li>';
        return;
    }

    filteredTodos.forEach(todo => {
        const li = document.createElement('li');
        li.className = `todo-item ${todo.isComplete ? 'completed' : ''}`;
        
        li.innerHTML = `
            <input type="checkbox" class="todo-checkbox" 
                   ${todo.isComplete ? 'checked' : ''} 
                   onchange="toggleTodo(${todo.id}, this.checked)">
            <span class="todo-text" ondblclick="enableEdit(this, ${todo.id})">${escapeHtml(todo.name)}</span>
            <div class="todo-actions">
                <button class="edit-btn" onclick="enableEdit(this.parentElement.previousElementSibling, ${todo.id})">Edit</button>
                <button class="delete-btn" onclick="deleteTodo(${todo.id})">Delete</button>
            </div>
        `;
        
        todoList.appendChild(li);
    });
}

function getFilteredTodos() {
    switch (currentFilter) {
        case 'active':
            return todos.filter(todo => !todo.isComplete);
        case 'completed':
            return todos.filter(todo => todo.isComplete);
        default:
            return todos;
    }
}

function updateStats() {
    const activeTodos = todos.filter(todo => !todo.isComplete);
    const count = activeTodos.length;
    todoCount.textContent = `${count} task${count !== 1 ? 's' : ''} remaining`;
}

function showAllTodos() {
    setActiveFilter('all');
    currentFilter = 'all';
    renderTodos();
}

function showActiveTodos() {
    setActiveFilter('active');
    currentFilter = 'active';
    renderTodos();
}

function showCompletedTodos() {
    setActiveFilter('completed');
    currentFilter = 'completed';
    renderTodos();
}

function setActiveFilter(filter) {
    filterButtons.forEach(btn => btn.classList.remove('active'));
    document.querySelector(`[onclick="show${filter.charAt(0).toUpperCase() + filter.slice(1)}Todos()"]`).classList.add('active');
}

async function clearCompleted() {
    const completedTodos = todos.filter(todo => todo.isComplete);
    
    if (completedTodos.length === 0) {
        alert('No completed tasks to clear!');
        return;
    }

    if (!confirm(`Are you sure you want to delete ${completedTodos.length} completed task(s)?`)) {
        return;
    }

    try {
        // Delete all completed todos
        const deletePromises = completedTodos.map(todo => 
            fetch(`${API_BASE_URL}/${todo.id}`, { method: 'DELETE' })
        );
        
        await Promise.all(deletePromises);
        
        // Update local state
        todos = todos.filter(todo => !todo.isComplete);
        renderTodos();
        updateStats();
    } catch (error) {
        showError('Failed to clear completed tasks: ' + error.message);
    }
}

function enableEdit(textElement, todoId) {
    const currentText = textElement.textContent;
    const input = document.createElement('input');
    input.type = 'text';
    input.value = currentText;
    input.className = 'todo-text';
    input.style.border = '1px solid #667eea';
    input.style.borderRadius = '3px';
    input.style.padding = '5px';

    function saveEdit() {
        const newText = input.value.trim();
        if (newText && newText !== currentText) {
            editTodo(todoId, newText);
        } else {
            textElement.textContent = currentText;
            textElement.style.display = 'block';
            input.remove();
        }
    }

    input.addEventListener('blur', saveEdit);
    input.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            saveEdit();
        } else if (e.key === 'Escape') {
            textElement.textContent = currentText;
            textElement.style.display = 'block';
            input.remove();
        }
    });

    textElement.style.display = 'none';
    textElement.parentNode.insertBefore(input, textElement.nextSibling);
    input.focus();
    input.select();
}

function showLoading() {
    todoList.innerHTML = '<li class="loading">Loading tasks...</li>';
}

function showError(message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error';
    errorDiv.textContent = message;
    
    // Remove any existing error messages
    const existingError = document.querySelector('.error');
    if (existingError) {
        existingError.remove();
    }
    
    // Insert error message at the top of the container
    const container = document.querySelector('.container');
    container.insertBefore(errorDiv, container.firstChild);
    
    // Auto-remove error after 5 seconds
    setTimeout(() => {
        errorDiv.remove();
    }, 5000);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}