// Base URL variable
var BaseUrl = "/CFT";

$(document).ready(function () {
    console.log("Document ready - login form initialized");

    // Password toggle
    $('#togglePassword').click(function () {
        console.log("Password toggle clicked");
        const passwordInput = $('#password');
        const icon = $(this).find('i');

        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passwordInput.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    // Forgot password click handler
    $('#forgotPassword').click(function (e) {
        e.preventDefault();
        alert('Please contact your administrator to reset your password.');
    });

    // Social login button handlers
    $('.social-btn.google').click(function () {
        alert('Google login would be implemented here.');
    });

    $('.social-btn.facebook').click(function () {
        alert('Facebook login would be implemented here.');
    });

    $('.social-btn.microsoft').click(function () {
        alert('Microsoft login would be implemented here.');
    });

    // Email validation on blur
    $('#email').on('blur', function () {
        console.log("Email field blurred");
        const email = $(this).val();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email && !emailRegex.test(email)) {
            $(this).css('border-color', 'var(--danger)');
            $(this).css('box-shadow', '0 0 0 3px rgba(231, 76, 60, 0.1)');
        } else {
            $(this).css('border-color', 'var(--border-color)');
            $(this).css('box-shadow', 'none');
        }
    });

    // Test if inputs are editable
    $('#email, #password').on('input', function () {
        console.log($(this).attr('id') + " input changed to:", $(this).val());
    });

    // Handle form submission
    $('#loginForm').on('submit', function (e) {
        e.preventDefault();
        console.log("Form submission prevented - AJAX will handle");

        // Get form data
        var email = $('#email').val();
        var password = $('#password').val();
        var token = $('input[name="__RequestVerificationToken"]').val();

        console.log("Email:", email);
        console.log("Password length:", password ? password.length : 0);

        // Client-side validation
        if (!email || !password) {
            showErrorMessage('Please enter both email and password');
            return;
        }

        // Email format validation
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showErrorMessage('Please enter a valid email address');
            return;
        }

        // Show loading state
        var loginBtn = $('#loginButton');
        var originalText = loginBtn.find('span').text();
        loginBtn.prop('disabled', true);
        loginBtn.find('span').text('Signing in...');
        loginBtn.css('opacity', '0.8');

        console.log("Sending AJAX request to:", BaseUrl + '/Login/Index');

        // AJAX call
        $.ajax({
            url: BaseUrl + '/Login/Index',
            type: 'POST',
            data: {
                Email: email,
                EmailId: email,
                Password: password,
                RememberMe: $('input[name="RememberMe"]').is(':checked')
            },
            headers: {
                'RequestVerificationToken': token,
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response, textStatus, xhr) {
                console.log("AJAX Success - Status:", textStatus);
                console.log("Response type:", typeof response);

                // Try to parse JSON
                var parsed = null;
                if (typeof response === 'object') {
                    parsed = response;
                    console.log("Parsed as object:", parsed);
                } else if (typeof response === 'string') {
                    console.log("Response is string, attempting to parse JSON");
                    try {
                        parsed = JSON.parse(response);
                        console.log("Successfully parsed JSON:", parsed);
                    } catch (e) {
                        console.log("Not JSON, likely HTML response");
                    }
                }

                // Check if force reset is required
                if (parsed && parsed.success && parsed.forceReset) {
                    console.log("Force reset required for email:", parsed.email);

                    // Hide login container
                    $('.login-container').hide();

                    // Set email in reset modal
                    $('#resetEmail').val(parsed.email || email);

                    // Show reset password modal
                    showResetPasswordModal();

                    return;
                }

                // If login successful without reset
                if (parsed && parsed.success) {
                    console.log("Login successful, redirecting to:", parsed.redirectUrl || (BaseUrl + '/Dashboard'));
                    loginBtn.find('span').text('Login Successful!');
                    setTimeout(function () {
                        window.location.href = parsed.redirectUrl || (BaseUrl + '/Dashboard');
                    }, 300);
                    return;
                }

                // If response is HTML (redirected)
                if (typeof response === 'string' && (response.includes('<!DOCTYPE') || response.includes('<html'))) {
                    console.log("HTML response detected, redirecting to dashboard");
                    window.location.href = BaseUrl + '/Dashboard';
                    return;
                }

                // Fallback error
                loginBtn.prop('disabled', false);
                loginBtn.find('span').text(originalText);
                loginBtn.css('opacity', '1');

                var message = 'Invalid email or password';
                if (parsed && parsed.message) {
                    message = parsed.message;
                }
                console.log("Login error:", message);
                showErrorMessage(message);
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:");
                console.error("Status:", status);
                console.error("Error:", error);
                console.error("Response:", xhr.responseText);

                loginBtn.prop('disabled', false);
                loginBtn.find('span').text(originalText);
                loginBtn.css('opacity', '1');

                if (xhr.responseJSON && xhr.responseJSON.message) {
                    showErrorMessage(xhr.responseJSON.message);
                } else if (xhr.status === 0) {
                    showErrorMessage('Network error. Please check your connection.');
                } else if (xhr.status === 500) {
                    showErrorMessage('Server error. Please try again later.');
                } else {
                    showErrorMessage('An error occurred during login. Please try again.');
                }
            }
        });
    });
});

// Function to show error messages
function showErrorMessage(message) {
    console.log("Showing error message:", message);

    // Remove existing error message
    $('.error-message').remove();

    // Add new error message
    var errorDiv = $('<div class="error-message"><i class="fas fa-exclamation-circle"></i> ' + message + '</div>');
    $('#loginForm').prepend(errorDiv);

    // Auto-hide after 5 seconds
    setTimeout(function () {
        errorDiv.fadeOut(500, function () {
            $(this).remove();
        });
    }, 5000);
}

// Function to show reset password modal
function showResetPasswordModal() {
    console.log("Showing reset password modal");
    $('#resetPasswordModal').css('display', 'block');
    $('#modalBackdrop').css('display', 'block');
    setTimeout(() => {
        $('#modalBackdrop').addClass('show');
        $('#resetPasswordModal').addClass('show');
    }, 10);
    $('body').css('overflow', 'hidden');
}

// Function to close reset password modal
function closeResetModal() {
    console.log("Closing reset password modal");
    $('#resetPasswordModal').removeClass('show');
    $('#modalBackdrop').removeClass('show');
    setTimeout(() => {
        $('#resetPasswordModal').css('display', 'none');
        $('#modalBackdrop').css('display', 'none');
        $('.login-container').show();
        // Clear reset form fields
        $('#newPassword').val('');
        $('#confirmPassword').val('');
        $('#resetError').addClass('d-none');
        $('#resetSuccess').addClass('d-none');
    }, 150);
    $('body').css('overflow', 'auto');
}

// Function to reset password
function resetPassword() {
    var email = $("#resetEmail").val();
    var newPwd = $("#newPassword").val();
    var confirmPwd = $("#confirmPassword").val();
    var token = $('input[name="__RequestVerificationToken"]').val();

    console.log("Resetting password for:", email);
    console.log("New password length:", newPwd.length);
    console.log("Confirm password length:", confirmPwd.length);

    // Clear previous messages
    $('#resetError').addClass('d-none');
    $('#resetSuccess').addClass('d-none');

    // Validation
    if (!newPwd || !confirmPwd) {
        $('#resetError').text('Please enter both password fields').removeClass('d-none');
        return;
    }

    if (newPwd.length < 8) {
        $('#resetError').text('Password must be at least 8 characters long').removeClass('d-none');
        return;
    }

    if (newPwd !== confirmPwd) {
        $('#resetError').text('Passwords do not match').removeClass('d-none');
        return;
    }

    // Show loading
    var updateBtn = $('.modal-footer .btn-primary');
    var cancelBtn = $('.modal-footer .btn-secondary');
    var originalText = updateBtn.text();

    updateBtn.prop('disabled', true);
    cancelBtn.prop('disabled', true);
    updateBtn.text('Updating...');

    console.log("Sending reset request to:", BaseUrl + '/Account/ResetPassword');

    $.ajax({
        url: BaseUrl + '/Login/ResetPassword',
        type: 'POST',
        data: {
            EmailId: email,
            Password: newPwd,
            ConfirmPassword: confirmPwd
        },
        headers: {
            'RequestVerificationToken': token,
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (res) {
            console.log("Reset password response:", res);
            if (res && res.success) {
                $('#resetSuccess').text('Password updated successfully! Redirecting to login...').removeClass('d-none');
                setTimeout(function () {
                    closeResetModal();
                    window.location.href = BaseUrl + "/Login/Index";
                }, 2000);
            } else {
                $('#resetError').text((res && res.message) || 'Failed to update password').removeClass('d-none');
                updateBtn.prop('disabled', false);
                cancelBtn.prop('disabled', false);
                updateBtn.text(originalText);
            }
        },
        error: function (xhr, status, error) {
            console.error("Reset password error:", error);
            console.error("Response:", xhr.responseText);

            var errorMsg = 'An error occurred. Please try again.';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMsg = xhr.responseJSON.message;
            } else if (xhr.status === 0) {
                errorMsg = 'Network error. Please check your connection.';
            }

            $('#resetError').text(errorMsg).removeClass('d-none');
            updateBtn.prop('disabled', false);
            cancelBtn.prop('disabled', false);
            updateBtn.text(originalText);
        }
    });
}

// Close modal when clicking on backdrop
$(document).on('click', '#modalBackdrop', function (e) {
    if (e.target === this) {
        closeResetModal();
    }
});

// Close modal with ESC key
$(document).on('keydown', function (e) {
    if (e.key === 'Escape' && $('#resetPasswordModal').is(':visible')) {
        closeResetModal();
    }
});