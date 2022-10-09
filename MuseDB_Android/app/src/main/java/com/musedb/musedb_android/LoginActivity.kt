package com.musedb.musedb_android

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.CheckBox
import com.sample.musedb_android.R

class LoginActivity : AppCompatActivity() {
    var isRegister = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)
    }

    fun LoginRegister(view: View) {

    }

    fun OnLoginRegisterChange(view: View) {
        if (view is CheckBox) {
            val checked: Boolean = view.isChecked
            isRegister = checked;
        }
    }
}