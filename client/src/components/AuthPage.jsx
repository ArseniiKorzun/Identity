import { useState } from 'react'
import { api } from '../api.js'

const emptyLogin = { login: '', password: '' }
const emptyRegister = { login: '', displayName: '', email: '', password: '', confirmPassword: '', city: '', favoriteGenre: '' }

export default function AuthPage({ onLogin }) {
  const [mode, setMode] = useState('login')
  const [login, setLogin] = useState(emptyLogin)
  const [register, setRegister] = useState(emptyRegister)
  const [message, setMessage] = useState(null)
  const [loading, setLoading] = useState(false)

  const isLogin = mode === 'login'

  const switchMode = (next) => {
    setMode(next)
    setMessage(null)
  }

  const submit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setMessage(null)
    try {
      if (isLogin) {
        const isValid = await api.login(login)
        if (!isValid) throw new Error('Invalid login or password.')
        onLogin({ name: login.login })
      } else {
        await api.register({
          ...register,
          city: register.city || null,
          favoriteGenre: register.favoriteGenre || null,
        })
        setLogin({ login: register.login, password: '' })
        setRegister(emptyRegister)
        setMode('login')
        setMessage({ type: 'success', text: 'Registration successful! You can log in now.' })
      }
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    } finally {
      setLoading(false)
    }
  }

  const field = (state, setState) => (name, props) => (
    <input
      name={name}
      value={state[name]}
      onChange={(e) => setState({ ...state, [name]: e.target.value })}
      {...props}
    />
  )
  const loginField = field(login, setLogin)
  const registerField = field(register, setRegister)

  return (
    <div className="auth-page">
      <form className="auth-form" onSubmit={submit}>
        <div className="brand">🎬 Films</div>
        <h1>{isLogin ? 'LOGIN' : 'REGISTER'}</h1>
        <p className="subtitle">{isLogin ? 'Welcome back! Sign in to see your films.' : 'Create an account to start your collection.'}</p>
        {isLogin ? (
          <>
            {loginField('login', { placeholder: 'Login', required: true, minLength: 5, maxLength: 50 })}
            {loginField('password', { type: 'password', placeholder: 'Password', required: true })}
          </>
        ) : (
          <>
            {registerField('login', { placeholder: 'Login (5-50 characters)', required: true, minLength: 5, maxLength: 50 })}
            {registerField('displayName', { placeholder: 'Display name (3-30 characters)', required: true, minLength: 3, maxLength: 30 })}
            {registerField('email', { type: 'email', placeholder: 'Email', required: true })}
            {registerField('password', { type: 'password', placeholder: 'Password', required: true })}
            {registerField('confirmPassword', { type: 'password', placeholder: 'Confirm password', required: true })}
            {registerField('city', { placeholder: 'City', maxLength: 60 })}
            {registerField('favoriteGenre', { placeholder: 'Favorite genre', maxLength: 50 })}
          </>
        )}

        {message && <p className={`message ${message.type}`}>{message.text}</p>}

        <div className="auth-buttons">
          {isLogin ? (
            <>
              <button type="submit" disabled={loading}>Login</button>
              <button type="button" className="secondary" onClick={() => switchMode('register')}>register</button>
            </>
          ) : (
            <>
              <button type="submit" disabled={loading}>Register</button>
              <button type="button" className="secondary" onClick={() => switchMode('login')}>back to login</button>
            </>
          )}
        </div>
      </form>
    </div>
  )
}
