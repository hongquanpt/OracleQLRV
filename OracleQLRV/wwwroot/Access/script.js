console.clear();

const loginBtn = document.getElementById('login');
const signupBtn = document.getElementById('signup');
const formInputLogin = document.getElementById('input-login')
const formInputRegister = document.getElementById('input-register')

loginBtn.addEventListener('click', (e) => {
	let parent = e.target.parentNode.parentNode;
	Array.from(e.target.parentNode.parentNode.classList).find((element) => {
		if(element !== "slide-up") {
			parent.classList.add('slide-up')
		}else{
			signupBtn.parentNode.classList.add('slide-up')
			parent.classList.remove('slide-up')
			formInputLogin.style.display= 'none'
			formInputRegister.style.display=''
		}
	});
});

signupBtn.addEventListener('click', (e) => {
	let parent = e.target.parentNode;
	Array.from(e.target.parentNode.classList).find((element) => {
		if(element !== "slide-up") {
			parent.classList.add('slide-up')
		}else{
			loginBtn.parentNode.parentNode.classList.add('slide-up')
			parent.classList.remove('slide-up')
			formInputLogin.style.display= ''
			formInputRegister.style.display='none'
			
		}
	});
});