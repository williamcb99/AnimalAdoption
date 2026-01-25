import { Route, Routes } from "react-router";
import { Layout } from "./components/Layout/Layout";
import { Homepage } from "./pages/Homepage/Homepage";

export const App = () => {
    return (
        <>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route index element={<Homepage />} />
                    <Route path="available-animals" />
                    <Route path="about-us" />
                    <Route path="login" />
                </Route>
            </Routes>
        </>
    );
}

export default App;