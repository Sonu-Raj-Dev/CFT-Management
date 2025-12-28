var BaseUrl = "/CFT";
var userModal;
var userTable;

$(document).ready(function () {

    userModal = new bootstrap.Modal(document.getElementById('userModal'));

    loadUsers();

    $('#btnSearch').click(function () {
        loadUsers();
    });

    $('#searchInput').keypress(function (e) {
        if (e.which === 13) {
            loadUsers();
            return false;
        }
    });
});

/**
 * Load users using DataTable
 */
function loadUsers() {

    var searchText = $('#searchInput').val();

    if ($.fn.DataTable.isDataTable('#usersTable')) {
        $('#usersTable').DataTable().destroy();
    }

    userTable = $('#usersTable').DataTable({
        processing: true,
        serverSide: false,
        searching: false,
        ordering: false,
        paging: true,
        pageLength: 10,

        ajax: {
            url: BaseUrl + '/CustomerMaster/GetDashBoardData',
            type: 'GET',
            data: {
                searchText: searchText
            },
            dataSrc: ''
        },
        columns: [
            { data: 'firstName' },
            { data: 'lastName' },
            { data: 'emailId' },
            { data: 'mobileNo' },
            { data: 'address' },
            //{ data: 'password' },
            {
                data: 'isActive',
                render: function (data) {
                    return data
                        ? '<span class="badge bg-success">Active</span>'
                        : '<span class="badge bg-danger">Inactive</span>';
                }
            },
            {
                "sortable": false, "width": "5%",
                "render": function (data, type, full, meta) {
                    return '<a class="action-icon " data-toggle="tooltip" data-placement="top" title="Edit" onclick="editUser(' + "'" + '' + full.encryptedParam + '' + "'" + ')"><i class="fas fa-pencil-alt " aria-hidden="true"></a>';
                },
                className: "text-center"
            }
        ]
    });
}

/**
 * Open Add User Modal
 */
function addUser() {
    $('#userForm')[0].reset();
    $('#userId').val('');
    $('#userModalTitle').text('Add New Customer');
    userModal.show();
}

/**
 * Save User (Create / Update)
 */

function saveUser() {

    var token = $('input[name="__RequestVerificationToken"]').val();

    var model = {
        Id: $('#userId').val() || 0,
        FirstName: $('#firstName').val(),
        LastName: $('#lastName').val(),
        EmailId: $('#emailId').val(),
        MobileNo: $('#mobileNo').val(),
        Address: $('#address').val(),
        IsActive: $('#isActive').is(':checked')
    };
    debugger;

    $.ajax({
        url: BaseUrl + '/CustomerMaster/Create',
        type: 'POST',
        data: model,
        headers: {
            'RequestVerificationToken': token
        },
        success: function (data) {

            userModal.hide();

            if (data.message === "CREATE") {
                showAutoCloseMessage("Customer created successfully");
            }
            else if (data.message === "UPDATE") {
                showAutoCloseMessage("Customer updated successfully");
            }
            else {
                showAutoCloseMessage("operation failed");
            }
        },
        error: function (xhr) {
            bootbox.alert(xhr.responseText || "Something went wrong");
        }
    });
}

/**
 * Edit User
 */
function editUser(encryptedParam) {

    $.ajax({
        url: BaseUrl + '/CustomerMaster/Create',
        type: 'GET',
        data: { id: encryptedParam },
        success: function (data) {
            console.log(data);
            $('#userId').val(data.id);
            $('#firstName').val(data.firstName);
            $('#lastName').val(data.lastName);
            $('#emailId').val(data.emailId);
            $('#mobileNo').val(data.mobileNumber);
            $('#address').val(data.address);
            $('#isActive').prop('checked', data.isActive);

            $('#password').val('');
            $('#userModalTitle').text('Edit Customer');

            userModal.show();
        },
        error: function () {
            alert('Failed to load user details.');
        }
    });

}


function showAutoCloseMessage(message) {
    var dialog = bootbox.dialog({
        message: message,
        closeButton: false,
        backdrop: true
    });

    setTimeout(function () {
        dialog.modal('hide');
        loadUsers();   // refresh grid after close
    }, 3000); // 3 seconds
}


