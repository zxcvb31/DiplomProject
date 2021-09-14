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
        passwordChck: '',
		action: 'register'} };


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
	  console.log(user);
      this.$emit('register-form', { type: 'register', data: user });
	  
	  fetch("/login", {
			method: "POST",
			redirect: 'follow',
			headers: {
				'Content-Type': 'application/json;charset=utf-8'
			},
			body: JSON.stringify(user)
		})
		.then(response => {
			if (response.status == 401)
			{
				alert("Данный email уже зарегистрирован!");
				window.location.href = "http://127.0.0.1:3000/login";
				
			}
			console.log(response.url);
			if (response.redirected) {
            window.location.href = response.url;
			}
            // HTTP 301 response
            // HOW CAN I FOLLOW THE HTTP REDIRECT RESPONSE?
        })
        .catch(function(err) {
            console.info(err + " url: " + url);
        });
	  
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
        password: '',
		action: 'signin'} };


  },
  methods: {
    handleForm() {
		let formvalue = Object.assign({}, this.user);
		this.resetFormValues();
		this.$emit('signin-form', { type: 'signin', data: formvalue });
	  
	  
		
		
		
		fetch("/login", {
			method: "POST",
			redirect: 'follow',
			headers: {
				'Content-Type': 'application/json;charset=utf-8'
			},
			body: JSON.stringify(formvalue)
		})
		.then(response => {
			//console.log(response.url);
			console.log(response.status);
			if (response.status == 401)
			{
				alert("Такой пользователь не найден");
				window.location.href = "http://127.0.0.1:3000/login";
				
			}
			if (response.redirected) {
            window.location.href = response.url;
			}
            // HTTP 301 response
            // HOW CAN I FOLLOW THE HTTP REDIRECT RESPONSE?
        })
        .catch(function(err) {
            console.info(err + " url: " + url);
        });
		//document.location.href = 'http://stackoverflow.com';
		
		
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







const app = new Vue({
  el: '#app',
  components: {
    register: registerComponent,
    signin: signInComponent },

  name: 'application',
  data() {
    return {
      feedback: {},
      currentComponent: 'signin' };

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