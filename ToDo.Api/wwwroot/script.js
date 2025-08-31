// API base URL - adjust if your API runs on a different port
const API_BASE_URL = '/todoitems';

// Global variables
let todos = [];
let currentFilter = 'all';
let availableTags = [];

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
            isComplete: false,
            description: '',
            priority: 2, // Medium priority
            tags: [],
            dueDate: null
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
        // Ensure tags is always an array
        if (!createdTodo.tags) {
            createdTodo.tags = [];
        }
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

async function updateTodo(id, updates) {
    try {
        const todo = todos.find(t => t.id === id);
        if (!todo) return;

        const updatedTodo = {
            ...todo,
            ...updates
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
        Object.assign(todo, updates);
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
        li.className = `todo-item ${todo.isComplete ? 'completed' : ''} priority-${getPriorityText(todo.priority)}`;
        
        const dueDateText = todo.dueDate ? formatDate(todo.dueDate) : '';
        const dueDateClass = todo.dueDate && isOverdue(todo.dueDate) && !todo.isComplete ? 'overdue' : '';
        
        li.innerHTML = `
            <input type="checkbox" class="todo-checkbox" 
                   ${todo.isComplete ? 'checked' : ''} 
                   onchange="toggleTodo(${todo.id}, this.checked)">
            <div class="todo-content">
                <div class="todo-header">
                    <span class="todo-text" ondblclick="enableEdit(this, ${todo.id}, 'name')">${escapeHtml(todo.name || '')}</span>
                    <div class="todo-meta">
                        <span class="priority-badge priority-${getPriorityText(todo.priority)}">${getPriorityText(todo.priority)}</span>
                        ${dueDateText ? `<span class="due-date ${dueDateClass}">${dueDateText}</span>` : ''}
                    </div>
                </div>
                ${todo.description ? `<div class="todo-description">${escapeHtml(todo.description)}</div>` : ''}
                <div class="todo-tags">
                    ${renderTags(todo.tags || [])}
                </div>
                <div class="todo-dates">
                    <small class="created-date">Created: ${formatDateTime(todo.createdDate)}</small>
                </div>
            </div>
            <div class="todo-actions">
                <button class="edit-btn" onclick="openEditModal(${todo.id})">Edit</button>
                <button class="delete-btn" onclick="deleteTodo(${todo.id})">Delete</button>
            </div>
        `;
        
        todoList.appendChild(li);
    });
}

function renderTags(tags) {
    if (!tags || !Array.isArray(tags) || tags.length === 0) return '';
    return tags.map(tag => `<span class="tag">${escapeHtml(tag)}</span>`).join('');
}

function getPriorityText(priority) {
    switch (priority) {
        case 1: return 'low';
        case 2: return 'medium';
        case 3: return 'high';
        default: return 'medium';
    }
}

function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString();
}

function formatDateTime(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleString();
}

function isOverdue(dueDateString) {
    if (!dueDateString) return false;
    const dueDate = new Date(dueDateString);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return dueDate < today;
}

function getFilteredTodos() {
    switch (currentFilter) {
        case 'active':
            return todos.filter(todo => !todo.isComplete);
        case 'completed':
            return todos.filter(todo => todo.isComplete);
        case 'high-priority':
            return todos.filter(todo => todo.priority === 3);
        case 'overdue':
            return todos.filter(todo => !todo.isComplete && todo.dueDate && isOverdue(todo.dueDate));
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

function showHighPriorityTodos() {
    setActiveFilter('high-priority');
    currentFilter = 'high-priority';
    renderTodos();
}

function showOverdueTodos() {
    setActiveFilter('overdue');
    currentFilter = 'overdue';
    renderTodos();
}

function setActiveFilter(filter) {
    filterButtons.forEach(btn => btn.classList.remove('active'));
    const activeButton = document.querySelector(`[onclick*="${filter}"]`);
    if (activeButton) {
        activeButton.classList.add('active');
    }
}

function openEditModal(todoId) {
    const todo = todos.find(t => t.id === todoId);
    if (!todo) return;

    const modal = document.getElementById('editModal');
    if (!modal) {
        createEditModal();
        return openEditModal(todoId);
    }

    // Populate form
    document.getElementById('editName').value = todo.name || '';
    document.getElementById('editDescription').value = todo.description || '';
    document.getElementById('editPriority').value = todo.priority || 2;
    document.getElementById('editDueDate').value = todo.dueDate || '';
    document.getElementById('editTags').value = (todo.tags || []).join(', ');
    
    modal.style.display = 'block';
    modal.dataset.todoId = todoId;
}

function createEditModal() {
    const modalHTML = `
        <div id="editModal" class="modal">
            <div class="modal-content">
                <div class="modal-header">
                    <h2>Edit Task</h2>
                    <span class="close" onclick="closeEditModal()">&times;</span>
                </div>
                <form id="editForm" onsubmit="saveEdit(event)">
                    <div class="form-group">
                        <label for="editName">Task Name:</label>
                        <input type="text" id="editName" required>
                    </div>
                    <div class="form-group">
                        <label for="editDescription">Description:</label>
                        <textarea id="editDescription" rows="3"></textarea>
                    </div>
                    <div class="form-group">
                        <label for="editPriority">Priority:</label>
                        <select id="editPriority">
                            <option value="1">Low</option>
                            <option value="2">Medium</option>
                            <option value="3">High</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="editDueDate">Due Date:</label>
                        <input type="date" id="editDueDate">
                    </div>
                    <div class="form-group">
                        <label for="editTags">Tags (comma-separated):</label>
                        <input type="text" id="editTags" placeholder="work, important, urgent">
                    </div>
                    <div class="form-actions">
                        <button type="button" onclick="closeEditModal()">Cancel</button>
                        <button type="submit">Save Changes</button>
                    </div>
                </form>
            </div>
        </div>
    `;
    
    document.body.insertAdjacentHTML('beforeend', modalHTML);
}

function closeEditModal() {
    const modal = document.getElementById('editModal');
    if (modal) {
        modal.style.display = 'none';
    }
}

function saveEdit(event) {
    event.preventDefault();
    
    const modal = document.getElementById('editModal');
    const todoId = parseInt(modal.dataset.todoId);
    
    const name = document.getElementById('editName').value.trim();
    if (!name) {
        alert('Task name is required!');
        return;
    }
    
    const description = document.getElementById('editDescription').value.trim();
    const priority = parseInt(document.getElementById('editPriority').value);
    const dueDate = document.getElementById('editDueDate').value || null;
    const tagsText = document.getElementById('editTags').value.trim();
    const tags = tagsText ? tagsText.split(',').map(tag => tag.trim()).filter(tag => tag) : [];
    
    const updates = {
        name,
        description,
        priority,
        dueDate,
        tags
    };
    
    updateTodo(todoId, updates);
    closeEditModal();
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

function enableEdit(textElement, todoId, field) {
    const currentText = textElement.textContent;
    const input = document.createElement('input');
    input.type = 'text';
    input.value = currentText;
    input.className = 'todo-text-edit';
    input.style.border = '1px solid #667eea';
    input.style.borderRadius = '3px';
    input.style.padding = '5px';
    input.style.width = '100%';

    function saveEdit() {
        const newText = input.value.trim();
        if (newText && newText !== currentText) {
            updateTodo(todoId, { [field]: newText });
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

// Close modal when clicking outside of it
window.onclick = function(event) {
    const modal = document.getElementById('editModal');
    if (event.target === modal) {
        closeEditModal();
    }
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