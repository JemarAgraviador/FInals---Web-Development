async function loadBooks() {
    const res = await fetch('/api/library/books');
    const books = await res.json();
    const tbody = document.getElementById('bookList');
    tbody.innerHTML = '';
    books.forEach(b => {
        const tr = document.createElement('tr');
        tr.innerHTML = `<td>${b.code}</td><td>${b.title}</td><td>${b.author}</td>`;
        tbody.appendChild(tr);
    });

    const res2 = await fetch('/api/library/borrowed');
    const borrowed = await res2.json();
    const tbody2 = document.getElementById('borrowedList');
    tbody2.innerHTML = '';
    borrowed.forEach(b => {
        const tr = document.createElement('tr');
        tr.innerHTML = `<td>${b.code}</td><td>${b.title}</td><td>${b.author}</td>`;
        tbody2.appendChild(tr);
    });
}

async function addBook() {
    const titleInput = document.getElementById('title');
    const authorInput = document.getElementById('author');
    const title = titleInput.value.trim();
    const author = authorInput.value.trim();
    if (!title || !author) return;
    titleInput.value = '';
    authorInput.value = '';
    const res = await fetch('/api/library/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, author })
    });
    alert(await res.text());
    loadBooks();
}

async function borrowBook() {
    const borrowInput = document.getElementById('borrowCode');
    const code = borrowInput.value.trim();
    if (!code) return;
    borrowInput.value = '';
    const res = await fetch('/api/library/borrow', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(code)
    });
    alert(await res.text());
    loadBooks();
}

async function returnBook() {
    const returnInput = document.getElementById('returnCode');
    const code = returnInput.value.trim();
    if (!code) return;
    returnInput.value = '';
    const res = await fetch('/api/library/return', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(code)
    });
    alert(await res.text());
    loadBooks();
}

loadBooks();
