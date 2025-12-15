$(document).ready(function () {
    // Handle form submission
    var BaseUrl = "/CFT";
    $('#loginForm').on('submit', function (e) {
        e.preventDefault();
        console.log('Form submitted'); // Debug log

        // Get form data
        var jsonData = {
            Email: $('#email').val(),
            Password: $('#password').val()
        };
        var token = $('input[name="__RequestVerificationToken"]').val();
        console.log('Form data:', jsonData); // Debug log

        // Client-side validation
        if (!jsonData.Email || !jsonData.Password) {
            showErrorMessage('Please enter both email and password');
            return;
        }

        // Email format validation
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(jsonData.Email)) {
            showErrorMessage('Please enter a valid email address');
            return;
        }

        // Show loading state
        var loginBtn = $('.login-btn');
        var originalText = loginBtn.text();
        loginBtn.prop('disabled', true);
        loginBtn.text('Logging in...');
        debugger;
        // AJAX call
        $.ajax({
            url: BaseUrl + '/Login/Index',
            type: 'POST',
            // include the antiforgery token in the form data (and keep header as safe fallback)
                data: {
                // send both names to match possible server expectations
                Email: $('#email').val(),
                EmailId: $('#email').val(),
                Password: $('#password').val()
            },
            headers: {
                'RequestVerificationToken': token,
                'X-Requested-With': 'XMLHttpRequest'
            },

            success: function (response, textStatus, xhr) {
                console.log('Response received:', response); // Debug log

                // Try to parse JSON (server may return JSON on AJAX)
                var parsed = null;
                if (typeof response === 'object') {
                    parsed = response;
                } else if (typeof response === 'string') {
                    try {
                        parsed = JSON.parse(response);
                    } catch (e) {
                        // not JSON — likely HTML (server redirected and returned full page)
                    }
                }

                if (parsed && parsed.success) {
                    // Login successful (JSON)
                    loginBtn.text('Login Successful!');
                    setTimeout(function () {
                        window.location.href = parsed.redirectUrl || (BaseUrl + '/Dashboard');
                    }, 300);
                    return;
                }

                // If response is HTML (server followed redirect and returned dashboard HTML),
                // perform a hard redirect so the browser renders the page normally.
                if (typeof response === 'string' && (response.indexOf('<!DOCTYPE') !== -1 || response.indexOf('<html') !== -1)) {
                    window.location.href = BaseUrl + '/Dashboard';
                    return;
                }

                // Fallback: treat as failure
                loginBtn.prop('disabled', false);
                loginBtn.text(originalText);

                var message = 'Invalid email or password';
                if (parsed && parsed.message) message = parsed.message;
                showErrorMessage(message);
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error:', xhr.responseText, status, error); // Debug log

                // Handle AJAX error
                loginBtn.prop('disabled', false);
                loginBtn.text(originalText);

                if (xhr.responseJSON && xhr.responseJSON.message) {
                    showErrorMessage(xhr.responseJSON.message);
                } else {
                    showErrorMessage('An error occurred during login. Please try again.');
                }
            }
        });
    });

    // Function to show error messages
    function showErrorMessage(message) {
        // Remove existing error message
        $('.error-message').remove();

        // Add new error message
        var errorDiv = $('<div class="error-message" style="color: red; text-align: center; margin-bottom: 15px; padding: 10px; background: rgba(255, 0, 0, 0.1); border-radius: 5px;">' + message + '</div>');
        $('#loginForm').prepend(errorDiv);

        // Auto-hide after 5 seconds
        setTimeout(function () {
            errorDiv.fadeOut();
        }, 5000);
    }
});