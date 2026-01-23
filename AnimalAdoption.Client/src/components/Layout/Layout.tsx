import { Outlet } from "react-router"
import { Header } from "../Header/Header"
import { Footer } from "../Footer/Footer"
import "./Layout.module.css"


export const Layout = () => {
    return (
    <>
        <Header />
        <main>
            <Outlet />
        </main>
        <Footer />
    </>
    )
}