// UserMaster.js - AJAX based User Master Management
var BaseUrl = "/CFT";
var userModal;
var currentSearchText = "";

$(function () {
    // Initialize Bootstrap modal
    userModal = new bootstrap.Modal(document.getElementById('userModal'));

    // Load users on page load
    loadUsers();

    // Search button click event
    $('#btnSearch').on('click', function () {
        searchUsers();
    });

    // Allow Enter key in search box
    $('#searchInput').on('keypress', function (e) {
        if (e.which == 13) {
            searchUsers();
            return false;
        }
    });

    // Form submission
    $('#userForm').on('submit', function (e) {
        e.preventDefault();
        saveUser();
    });
});

/**
 * Load all users with optional search filter
 */
function loadUsers() {
    var searchText = $('#searchInput').val().trim();
    
    $.ajax({
        url: BaseUrl + '/UserMaster/GetDashBoardData',
        type: 'GET',
        data: { searchtext: searchText },
        dataType: 'json',
        success: function (data) {
            populateTable(data);
        },
        error: function (xhr, status, error) {
            console.error('Error loading users:', error);
            showAlert('Error loading users. Please try again.', 'danger');
        }
    });
}

/**
 * Search users by text
 */
function searchUsers() {
    loadUsers();
}

/**
 * Populate the table with user data
 */
function populateTable(data) {
    var tbody = $('#tableBody');
    tbody.empty();

    if (data && data.length > 0) {
        $.each(data, function (index, user) {
            var statusBadge = user.isActive 
                ? '<span class="badge bg-success">Active</span>' 
                : '<span class="badge bg-secondary">Inactive</span>';
            
            var row = '<tr>' +
                '<td>' + (user.firstName || '') + '</td>' +
                '<td>' + (user.lastName || '') + '</td>' +
                '<td>' + (user.emailId || '') + '</td>' +
                '<td>' + statusBadge + '</td>' +
                '<td>' +
                    '<button class="btn btn-sm btn-primary" onclick="editUser(' + user.id + ')">' +
                        '<i class="bi bi-pencil"></i> Edit' +
                    '</button> ' +
                    '<button class="btn btn-sm btn-danger" onclick="deleteUser(' + user.id + ')">' +
                        '<i class="bi bi-trash"></i> Delete' +
                    '</button>' +
                '</td>' +
                '</tr>';
            tbody.append(row);
        });
    } else {
        tbody.append('<tr><td colspan="5" class="text-center text-muted">No users found.</td></tr>');
    }
}

/**
 * Add new user - Open modal for create
 */
function addUser() {
    // Reset form
    $('#userForm')[0].reset();
    $('#userId').val('0');
    $('#encryptedId').val('');
    
    // Update modal title
    $('#userModalTitle').text('Add New User');
    
    // Show modal
    userModal.show();
}

/**
 * Edit user - Load user data and open modal
 */
function editUser(userId) {
    // For now, redirect to traditional create page for editing
    // This can be enhanced to load data via AJAX and populate the modal
    $.ajax({
        url: BaseUrl + '/UserMaster/Create?id=' + encodeURIComponent(userId),
        type: 'GET',
        success: function (data) {
            // For edit functionality, we'll use the traditional form page
            window.location.href = BaseUrl + '/UserMaster/Create?id=' + encodeURIComponent(userId);
        }
    });
}

/**
 * Delete user
 */
function deleteUser(userId) {
    if (confirm('Are you sure you want to delete this user?')) {
        $.ajax({
            url: BaseUrl + '/UserMaster/DeleteUser',
            type: 'POST',
            data: { id: userId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    showAlert('User deleted successfully!', 'success');
                    loadUsers();
                } else {
                    showAlert('Error deleting user.', 'danger');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error deleting user:', error);
                showAlert('Error deleting user.', 'danger');
            }
        });
    }
}

/**
 * Save user via AJAX (Create/Update via modal)
 */
function saveUser() {
    // Validate required fields
    var firstName = $('#firstName').val().trim();
    var emailId = $('#emailId').val().trim();
    var password = $('#password').val().trim();

    if (!firstName) {
        showAlert('First Name is required.', 'warning');
        return;
    }
    
    if (!emailId) {
        showAlert('Email is required.', 'warning');
        return;
    }

    if (!password) {
        showAlert('Password is required.', 'warning');
        return;
    }

    var userData = {
        id: parseInt($('#userId').val()) || 0,
        firstName: firstName,
        lastName: $('#lastName').val().trim(),
        emailId: emailId,
        mobileNo: $('#mobileNo').val().trim(),
        password: password,
        isActive: $('#isActive').is(':checked'),
        roleId: 0,
        employeeType: 0,
        createdBy: 0
    };

    $.ajax({
        url: BaseUrl + '/UserMaster/Create',
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(userData),
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success || response.message === 'CREATE' || response.message === 'UPDATE') {
                var message = response.message === 'CREATE' 
                    ? 'User created successfully!' 
                    : 'User updated successfully!';
                showAlert(message, 'success');
                userModal.hide();
                loadUsers();
            } else {
                showAlert('Error saving user.', 'danger');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error saving user:', error);
            if (xhr.responseJSON && xhr.responseJSON.message) {
                showAlert('Error: ' + xhr.responseJSON.message, 'danger');
            } else {
                showAlert('Error saving user. Please try again.', 'danger');
            }
        }
    });
}

/**
 * Show alert message
 */
function showAlert(message, type) {
    var alertHtml = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
        message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
        '</div>';
    
    // Prepend alert to page (you can also create a dedicated alert container)
    $(alertHtml).prependTo('.container-fluid').delay(5000).fadeOut(function () {
        $(this).remove();
    });
}