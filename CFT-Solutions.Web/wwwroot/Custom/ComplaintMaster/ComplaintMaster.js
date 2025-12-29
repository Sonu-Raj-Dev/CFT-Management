// Global variables
var BaseUrl = "/CFT";

// Show/hide error messages
function showFieldError(fieldId, show) {
    $('#' + fieldId).toggle(show);
}

// Validate individual field
function validateField(fieldId, value) {
    if (!value || value.trim() === '') {
        showFieldError(fieldId + 'Error', true);
        return false;
    }
    showFieldError(fieldId + 'Error', false);
    return true;
}

// Validate all required fields
function validateAllFields() {
    let isValid = true;

    // Validate customer
    if (!validateField('customer', $('#customerid').val())) {
        isValid = false;
    }

    // Validate nature of complaint
    if (!validateField('nature', $('#natureofcomplaintId').val())) {
        isValid = false;
    }

    // Validate engineer
    if (!validateField('engineer', $('#engineerid').val())) {
        isValid = false;
    }

    // Validate complaint details
    if (!validateField('details', $('#complaintdetails').val())) {
        isValid = false;
    }

    return isValid;
}

// Show success message
function showSuccess(message) {
    const dialog = bootbox.alert({
        title: "<i class='fa fa-check-circle text-success'></i> Success",
        message: message,
        centerVertical: true,
        backdrop: false
    });

    setTimeout(function () {
        dialog.modal('hide');
    }, 1500);
}


// Make this function globally available

function onCustomerChange(customerId) {
    if (!customerId) {
        // Clear all fields if no customer selected
        $('#txtCustomerName').val('');
        $('#txtMobileNumber').val('');
        $('#txtEmail').val('');
        $('#txtAddress').val('');

        // Update hidden fields
        $('input[name="CustomerName"]').val('');
        $('input[name="MobileNumber"]').val('');
        $('input[name="CustomerEmail"]').val('');
        $('input[name="Address"]').val('');
        return;
    }

    // Fetch customer data via AJAX
    $.ajax({
        url: BaseUrl + "/ComplaintMaster/GetCustomerById",
        type: 'GET',
        data: { customerId: customerId },
        success: function (data) {
            if (data) {
                // Update visible fields
                $('#txtCustomerName').val(data.customerName || '');
                $('#txtMobileNumber').val(data.mobileNumber || '');
                $('#txtEmail').val(data.email || '');
                $('#txtAddress').val(data.address || '');

                // Update hidden fields for form submission
                $('input[name="CustomerName"]').val(data.customerName || '');
                $('input[name="MobileNumber"]').val(data.mobileNumber || '');
                $('input[name="CustomerEmail"]').val(data.email || '');
                $('input[name="Address"]').val(data.address || '');
            }
        },
        error: function () {
            alert('Error loading customer details');
        }
    });
}

// Show error message
function showError(message) {
    bootbox.alert({
        title: "<i class='fa fa-exclamation-triangle text-danger'></i> Error",
        message: message,
        centerVertical: true
    });
}

// Show loading state on button
function showLoading(button, isLoading) {
    if (isLoading) {
        $(button).prop('disabled', true).addClass('btn-loading');
    } else {
        $(button).prop('disabled', false).removeClass('btn-loading');
    }
}

// Reset form
function resetForm() {
    $('#customerId').val('');
    $('#txtCustomerName').val('');
    $('#txtMobileNumber').val('');
    $('#txtEmail').val('');
    $('#txtAddress').val('');
    $('#natureOfComplaintId').val('');
    $('#engineerId').val('');
    $('#complaintDetails').val('');

    // Clear hidden fields
    $('#hdnCustomerName').val('');
    $('#hdnMobileNumber').val('');
    $('#hdnCustomerEmail').val('');
    $('#hdnAddress').val('');

    // Hide all errors
    $('.text-danger').hide();
}

// Remove this duplicate function since we have window.onCustomerChange
// $('#customerId').change(function () { ... }); // Comment out or remove this

// Save or Submit complaint
function saveComplaint(actionType) {
    debugger;
    // Validate all fields
    if (!validateAllFields()) {
        showError('Please fill all required fields');
        return;
    }

    // Create FormData object correctly
    const formData = new FormData();

    // Append all form data
    formData.append('Id', $('#Id').val() || 0);
    formData.append('CustomerId', $('#customerid').val());
    formData.append('CustomerName', $('#txtCustomerName').val());
    formData.append('MobileNumber', $('#txtMobileNumber').val());
    formData.append('CustomerEmail', $('#txtEmail').val());
    formData.append('Address', $('#txtAddress').val());
    formData.append('NatureOfComplaintId', $('#natureofcomplaintId').val());
    formData.append('EngineerId', $('#engineerid').val());
    formData.append('ComplaintDetails', $('#complaintdetails').val());
    formData.append('ActionType', actionType);

    // Show loading on appropriate button
    const submitButton = actionType === 'SaveDraft' ? $('#btnSaveDraft') : $('#btnSubmit');
    showLoading(submitButton, true);

    // AJAX call with FormData - CORRECT WAY
    $.ajax({
        url: BaseUrl + '/ComplaintMaster/Create',
        type: 'POST',
        data: formData, // Pass FormData directly
        processData: false, // Don't process the data
        contentType: false, // Don't set content type
        success: function (response) {
            debugger;
            showLoading(submitButton, false);

            // Handle response
            handleAjaxResponse(response, actionType, submitButton);
        },
        error: function (xhr, status, error) {
            showLoading(submitButton, false);
            handleAjaxError(xhr);
        }
    });
}

// Helper function to handle success response
function handleAjaxResponse(response, actionType) {

    if (response.message === "MODELERROR") {

        let errorMessage = "Validation errors:<br>";

        if (response.errorlist) {
            response.errorlist.split('#').forEach(err => {
                if (err.trim()) {
                    const parts = err.split(',');
                    if (parts.length >= 2) {
                        errorMessage += `• ${parts[1]}<br>`;
                    }
                }
            });
        } else {
            errorMessage = 'Validation failed. Please check your input.';
        }

        showError(errorMessage);
        return;
    }

    if (response.success || response.message === "CREATE" || response.message === "UPDATE") {
        forceRemoveOverlays();
        showSuccess(
            actionType === 'SaveDraft'
                ? 'Complaint saved as draft successfully!'
                : 'Complaint submitted successfully!'
        );

        setTimeout(function () {
            window.location.href = BaseUrl + '/ComplaintMaster/Index';
        }, 1500);

    } else {
        showError(response.message || 'Failed to save complaint');
    }
}

// Helper function to handle errors
function handleAjaxError(xhr) {
    if (xhr.responseJSON?.message) {
        showError(xhr.responseJSON.message);
    } else if (xhr.status === 401) {
        showError('Session expired. Please login again.');
    } else if (xhr.status === 403) {
        showError('You do not have permission to perform this action.');
    } else {
        showError('An unexpected error occurred. Please try again.');
    }
}
function forceRemoveOverlays() {
    $('.modal-backdrop').remove();
    $('body').removeClass('modal-open');
    $('#loader, .loader, .loading-overlay').hide(); // adjust to your loader id/class
}


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
            url: BaseUrl + '/ComplaintMaster/GetDashBoardData',
            type: 'GET',
            data: {
                searchText: searchText
            },
            dataSrc: ''
        },  
        columns: [
            { data: 'complaintCode' },
            { data: 'customerName' },
            { data: 'complaintDetails' },
            { data: 'mobileNumber' },
            { data: 'statusName' },
            { data: 'natureOfComplaint' },
            { data: 'engineerName' },
            { data: 'createdDateStr' },
            { data: 'createdByUser' },
            //{ data: 'password' },
            //{
            //    data: 'isActive',
            //    render: function (data) {
            //        return data
            //            ? '<span class="badge bg-success">Active</span>'
            //            : '<span class="badge bg-danger">Inactive</span>';
            //    }
            //},
            {
                "sortable": false, "width": "5%",
                "render": function (data, type, full, meta) {
                    console.log(full);
                    if (full.statusId != 3) {
                        return '<a class="action-icon " data-toggle="tooltip" data-placement="top" title="Edit" onclick="editComplaint(' + "'" + '' + full.encryptedParam + '' + "'" + ')"><i class="fas fa-pencil-alt " aria-hidden="true"></a>';
                    }
                    else {
                        return '<div class="btn-group">' +
                            '<a class="btn btn-sm" data-toggle="tooltip" data-placement="top" title="View" onclick="viewComplaint(\'' + full.encryptedParam + '\')">' +
                            '<i class="fas fa-eye"></i>' +
                            '</a>' +
                            '</div>';
                    }
                },
                className: "text-center"
            }
        ]
    });
}

function editComplaint(id) {
    // Edit mode - IsView = false
    var searchText = $('#searchInput').val();

    if ($.fn.DataTable.isDataTable('#usersTable')) {
        $('#usersTable').DataTable().destroy();
    }

    window.location.href = `${BaseUrl}/ComplaintMaster/Create?Id=${(id)}&IsView=false`;
}

function viewComplaint(id) {
    // View mode - IsView = true
    var searchText = $('#searchInput').val();

    if ($.fn.DataTable.isDataTable('#usersTable')) {
        $('#usersTable').DataTable().destroy();
    }

    window.location.href = `${BaseUrl}/ComplaintMaster/Create?Id=${(id)}&IsView=true`;
}
// Initialize page
$(document).ready(function () {

    loadUsers();
    $('#btnSaveDraft').click(function (e) {
        e.preventDefault();
        saveComplaint('SaveDraft');
    });
    $('#customerid').change(function () {
        onCustomerChange($(this).val());
    });
    // Submit button click
    $('#btnSubmit').click(function (e) {
        e.preventDefault();
        saveComplaint('Submit');
    });

    // Real-time validation
    $('#customerId, #natureOfComplaintId, #engineerId').change(function () {
        const fieldId = $(this).attr('id').replace('Id', '');
        validateField(fieldId, $(this).val());
    });

    $('#complaintDetails').on('input', function () {
        validateField('details', $(this).val());
    });

    // Close alert buttons
    $('.alert .btn-close').click(function () {
        $(this).closest('.alert').fadeOut();
    });
});
$('#btnUpdateStatus').click(function (e) {
    e.preventDefault();
    debugger;
    // Get complaint ID
    var complaintId = $('#complaintId').val();
    var currentStatusId = $('#currentStatusId').val();

    if (!complaintId || complaintId == '0') {
        showError('Invalid complaint ID');
        return;
    }

    // Show status selection modal
    UpdateStatus(complaintId);
});
function UpdateStatus(complaintId) {
    debugger;
    // Load status options via AJAX
    $.ajax({
        url: BaseUrl + '/ComplaintMaster/UpdateComplaintStatusById',
        type: 'POST',
        data: { complaintId: complaintId },
        beforeSend: function () {
            showLoading(true);
        },
        success: function (statusOptions) {
            hideLoading();
            window.location.href = BaseUrl+"/ComplaintMaster/Index"
        }
    })
}