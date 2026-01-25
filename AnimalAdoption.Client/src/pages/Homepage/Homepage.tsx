import { Section } from "../../components/Section/Section";
import styles from "../../components/Section/Section.module.css";
import { AnimalVideo } from "../../components/AnimalVideo/AnimalVideo";

export const Homepage = () => {
    return (
        <>
            <Section className={styles["hero-section"]}>
                <AnimalVideo isRandom={true}/>      
            </Section>
        </>
    );
}