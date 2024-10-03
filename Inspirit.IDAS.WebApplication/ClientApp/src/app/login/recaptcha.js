<script src="https://www.google.com/recaptcha/api.js?render=6LdPoWkfAAAAAEnCQgt-T0tukRM0GSPu_mRo7CpR"></script>

function gettoken(e) {
    e.preventDefault();
    grecaptcha.ready(function() {
      grecaptcha.execute('6LdPoWkfAAAAAEnCQgt-T0tukRM0GSPu_mRo7CpR', {action: 'loginpage'}).then(function(token) {
          // Add your logic to submit to your backend server here.
      });
    });
  }

   grecaptcha.ready(function() {
    grecaptcha.execute('6LdPoWkfAAAAAEnCQgt-T0tukRM0GSPu_mRo7CpR', {action: 'homepage'}).then(function(token) {
       document.getElementById("googletoken").value= token;
    })});