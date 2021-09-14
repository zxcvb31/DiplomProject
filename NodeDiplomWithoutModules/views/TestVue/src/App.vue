<template>
<html lang="en" >
<head>
  <meta charset="UTF-8">
  <title>CodePen - vue.js | register &amp; login form</title>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/normalize/5.0.0/normalize.min.css">
<link rel="stylesheet" href="css/VueFormastyle.css">

</head>
<body>
<!-- partial:index.partial.html -->
<div id="app" v-cloak>

	  <div class="actions">
			<button :class='[{ active: isDisabled("register") }]' 
				 @click.prevent='setComponent("register")'>Register
			</button> 
			<button :class='[{ active: isDisabled("signin") }]' 
				 @click.prevent='setComponent("signin")'>Signin
			</button>
	  </div>

	  <transition name='form' mode='out-in'>
			<keep-alive>
				 <component :feedback='feedback' :is="currentComponent"
				 @register-form='handleForm' @signin-form='handleForm'></component>
			</keep-alive>
	  </transition>

 </div>

 <!-- Register Template -->
 <template id="registerTemplate">
	  <form @submit.prevent='onSubmit' ref='form' action="" class='register-form'>
			<h2>Register</h2>
			<div class="form-group" >
				 <label for="firstname">Prénom</label>
				 <input required type="text" v-model.trim='user.firstname' id='firstname' placeholder="Prénom">
			</div>
			<div class="form-group">
				 <label for="lastname">Nom</label>
				 <input required type="text" v-model.trim='user.lastname' id='lastname' placeholder="Nom">
			</div>
			<div class="form-group">
				 <label for="email">Adresse électronique</label>
				 <input required type="email" v-model.trim='user.email' id='email' placeholder="Adresse électronique">
			</div>
			<div class="form-group">
				 <label for="password">Mot de passe</label>
				 <input required type="password" v-model='user.password' placeholder="Mot de passe" id='password'>
			</div>
			<div class="form-group">
				 <label for="passwordcheck">Vérification du mot de passe</label>
				 <input required type="password" v-model='user.passwordChck' placeholder="Vérification du mot de passe" id='passwordcheck'>
			</div>
			<input type="submit" :disabled='!isFormValid' value='Register'>
	  </form>
 </template>

 <!-- Signin Template -->
 <template id="signinTemplate">
	  <form ref='form' @submit.prevent='handleForm' action="" class='signin-form'>
			<h2>Signin</h2>
			<div class="form-group">
				 <label for="email">Adresse électronique</label>
				 <input required v-model='user.email' type="email" id='email' placeholder="Adresse électronique">
			</div>
			<div class="form-group">
				 <label for="password">Mot de passe</label>
				 <input required v-model='user.password' type="password" id='password' placeholder="Mot de passe">
			</div>
			<input :disabled='!isFormValid' type="submit" value="Signin">
	  </form>
 </template>

 <!-- Feedback Template -->
 <template id="feedbackTemplate">
	  <div class="feedback">
			<header>
				 <h2>{{ title }}</h2>
			</header>
			<div v-if='feedback.type === "register"'>
				<p>Bienvenue <strong>{{ feedback.data | name }}</strong>.</p>
				<p> Un email vient d'être envoyé à l'adresse {{ feedback.data | email }} afin de compléter ton inscription.</p>
			</div>
			<div v-else>
				 <p>Vous allez être redirigé(e) dans quelques instants...</p>
			</div>
	  </div>
 </template>
<!-- partial -->
  <script src='https://cdnjs.cloudflare.com/ajax/libs/vue/2.5.3/vue.min.js'></script><script  src="JS/VueFormascript.js"></script>

</body>
</html>
</template>
 
<script>
const emailRegex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;

const registerComponent = {
  template: '#registerTemplate',
  name: 'RegisterComponent',
  data() {
    return {
      user: {
        firstname: '',
        lastname: '',
        email: '',
        password: '',
        passwordChck: '' } };


  },
  computed: {
    isFormValid() {
      return (
        this.isValid('firstname') &&
        this.isValid('lastname') &&
        this.isValid('email') &&
        this.isValid('password') &&
        this.isValid('passwordChck'));

    } },

  methods: {
    isValid(prop) {
      switch (prop) {
        case 'firstname':
          return this.user.firstname.length >= 2;
          break;
        case 'lastname':
          return this.user.lastname.length >= 2;
          break;
        case 'email':
          return emailRegex.test(this.user.email);
          break;
        case 'password':
          return this.user.password.length >= 6;
          break;
        case "passwordChck":
          return this.user.password === this.user.passwordChck;
          break;
        default:
          return false;}

    },
    resetUser() {
      this.user.firstname = '';
      this.user.lastname = '';
      this.user.email = '';
      this.user.password = '';
      this.user.passwordChck = '';
    },
    onSubmit() {
      let user = Object.assign({}, this.user);
      this.resetUser();
      this.$emit('register-form', { type: 'register', data: user });
    } },

  mounted() {
    let element = this.$el.querySelector('#passwordcheck');
    element.addEventListener('blur', () => {
      if (!this.isValid('passwordChck')) {
        element.classList.add('invalid');
      } else {
        element.classList.remove('invalid');
      }
    });
  } };


const signInComponent = {
  template: '#signinTemplate',
  name: 'SigninComponent',
  data() {
    return {
      user: {
        email: '',
        password: '' } };


  },
  methods: {
    handleForm() {
      let formvalue = Object.assign({}, this.user);
      this.resetFormValues();
      this.$emit('signin-form', { type: 'signin', data: formvalue });
    },
    resetFormValues() {
      this.user.email = '';
      this.user.password = '';
    },
    isValid(prop) {
      switch (prop) {
        case 'email':
          return emailRegex.test(this.user.email);
          break;
        case 'password':
          return this.user.password.length >= 6;
          break;
        default:
          return false;}

    } },

  computed: {
    isFormValid() {
      return this.isValid('email') && this.isValid('password');
    } } };



const feedbackComponent = {
  template: '#feedbackTemplate',
  name: "FeedbackComponent",
  filters: {
    email(input) {
      if (input.email) {
        return input.email;
      } else {
        return '';
      }
    },
    name(input) {
      return input.firstname ? input.firstname : '';
    } },

  data() {return {};},
  props: ['feedback'],
  computed: {
    title() {
      return this.feedback.type === 'signin' ?
      'Authentification effectuée' : 'Inscription';
    } } };



const app = new Vue({
  el: '#app',
  components: {
    register: registerComponent,
    signin: signInComponent,
    feedback: feedbackComponent },

  name: 'application',
  data() {
    return {
      feedback: {},
      currentComponent: 'register' };

  },
  methods: {
    handleForm(data) {
      this.feedback = data;
      setTimeout(() => {
        this.setComponent('feedback');
      }, 280);
    },
    isDisabled(btnName) {
      return this.currentComponent === btnName;
    },
    setComponent(componentName) {
      if (this.currentComponent !== componentName) {
        this.currentComponent = componentName;
      }
    } } });
</script>
 
<style>
@import url("https://fonts.googleapis.com/css?family=PT+Sans");
*, *::before, *::after {
  box-sizing: border-box;
}

:root {
  --brandColor:#008b8b;
  --brandColorDark:#057272;
}

[v-cloak] {
  opacity: 0;
}

body {
  padding: 0;
  margin: 0;
  font-family: "PT Sans", sans-serif;
  background: #e0e0e0;
}

#app {
  border-top: 0.5em solid var(--brandColor);
  max-width: 800px;
  margin: 0 auto;
  position: absolute;
  top: 50%;
  left: 50%;
  width: 96%;
  transform: translate(-50%, -50%);
  padding: 2em 3em 1em;
  background: white;
  overflow: hidden;
  box-shadow: 0 10px 6px -6px rgba(0, 0, 0, 0.2);
  -webkit-animation: enterFromBottom 0.7s 0.3s ease-out both;
          animation: enterFromBottom 0.7s 0.3s ease-out both;
}
@media screen and (max-width: 500px) {
  #app {
    padding: 2em 1em 1em;
  }
}

.actions button {
  all: unset;
  display: inline-block;
  padding: 1em;
  letter-spacing: 0.05em;
  font-size: 14px;
  cursor: pointer;
  border: 1px solid;
  color: white;
  border: none;
  background: var(--brandColor);
  transition: 250ms;
  margin: 0 0.2em 0 0;
  opacity: 0.4;
}
.actions button.active {
  opacity: 1;
  background: var(--brandColorDark);
}

.form-enter {
  transform: translateY(-1em);
  opacity: 0;
}
.form-leave-to {
  transform: scale(0.95);
  opacity: 0;
}
.form-enter-active {
  transition: 0.3s ease-out;
}
.form-leave-active {
  transition: 0.3s ease;
}

form h2, header h2 {
  text-align: center;
  color: var(--brandColor);
}

.register-form, .signin-form {
  margin: 2em 0;
  padding: 1em;
}

.form-group {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  padding: 8px;
}
.form-group label {
  flex: 1;
  text-align: right;
  margin-right: 2em;
}
.form-group input {
  font-size: inherit;
  border: none;
  background: whitesmoke;
  font-family: inherit;
  padding: 0.4em;
  flex: 1.5;
}
.form-group input.invalid {
  border: 1px solid tomato;
}

.feedback {
  padding: 1em;
}
.feedback p {
  line-height: 1.4;
  max-width: 50ch;
  margin: 10px auto;
  text-align: center;
}

form input[type=submit] {
  display: block;
  margin: 2em 0 2em auto;
  padding: 0.6em 1em;
  font-size: inherit;
  cursor: pointer;
  background: var(--brandColor);
  color: white;
  border: none;
}
form input[type=submit]:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}
form input[type=submit]:hover {
  background: var(--brandColorDark);
}

@-webkit-keyframes enterFromBottom {
  0% {
    opacity: 0;
    transform: translate(-50%, -45%);
  }
  100% {
    opacity: 1;
    transform: translate(-50%, -50%);
  }
}

@keyframes enterFromBottom {
  0% {
    opacity: 0;
    transform: translate(-50%, -45%);
  }
  100% {
    opacity: 1;
    transform: translate(-50%, -50%);
  }
}
</style>